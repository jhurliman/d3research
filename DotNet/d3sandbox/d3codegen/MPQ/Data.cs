/*
 * Copyright (C) 2011 mooege project
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CrystalMpq;
using Gibbed.IO;
using Mooege.Core.GS.Common.Types.SNO;
using System.Linq;
using Mooege.Common.Storage;

namespace Mooege.Common.MPQ
{
    public class Data : MPQPatchChain
    {
        public Dictionary<SNOGroup, ConcurrentDictionary<int, Asset>> Assets = new Dictionary<SNOGroup, ConcurrentDictionary<int, Asset>>();
        public readonly Dictionary<SNOGroup, Type> Parsers = new Dictionary<SNOGroup, Type>();
        private readonly List<Task> _tasks = new List<Task>();

        private int parsedTasks = 0;
        
        private static readonly SNOGroup[] PatchExceptions = new[] { SNOGroup.TimedEvent, SNOGroup.Script, SNOGroup.AiBehavior, SNOGroup.AiState, SNOGroup.Conductor, SNOGroup.FlagSet, SNOGroup.Code, SNOGroup.Worlds, SNOGroup.LevelArea };

        // Only load a subset of all the SNO files
        private static readonly HashSet<SNOGroup> LoadGroups = new HashSet<SNOGroup>
        {
            SNOGroup.Actor, SNOGroup.GameBalance, SNOGroup.Monster, 
            SNOGroup.Power, SNOGroup.Scene, SNOGroup.SceneGroup
        };

        public Data()
            : base(new List<string> { "CoreData.mpq", "ClientData.mpq" }, "/base/d3-update-base-(?<version>.*?).mpq")
        { }

        public void Init()
        {
            this.InitCatalog(); // init asset-group dictionaries and parsers.
            this.LoadCatalogs(); // process the assets.
        }

        private void InitCatalog()
        {
            foreach (SNOGroup group in Enum.GetValues(typeof(SNOGroup)))
            {
                this.Assets.Add(group, new ConcurrentDictionary<int, Asset>());
            }

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(FileFormat))) continue;
                var attributes = (FileFormatAttribute[])type.GetCustomAttributes(typeof(FileFormatAttribute), false);
                if (attributes.Length == 0) continue;

                Parsers.Add(attributes[0].Group, type);
            }
        }

        private void LoadCatalogs()
        {
            this.LoadCatalog("CoreTOC.dat", false, LoadGroups); // as of patch beta patch 7841, blizz renamed TOC.dat as CoreTOC.dat
            //this.LoadCatalog("TOC.dat", true, LoadGroupsSet); // used for reading assets patched to zero bytes and removed from mainCatalog file.  
        }

        private void LoadCatalog(string fileName, bool useBaseMPQ = false, HashSet<SNOGroup> groupsToLoad = null)
        {
            var catalogFile = this.GetFile(fileName, useBaseMPQ);
            this._tasks.Clear();

            if (catalogFile == null)
            {
                Logger.Error("Couldn't load catalog file: {0}.", fileName);
                return;
            }

            var stream = catalogFile.Open();
            var assetsCount = stream.ReadValueS32();

            var timerStart = DateTime.Now;

            int count = 0;

            // read all assets from the catalog first and process them (ie. find the parser if any available).
            while (stream.Position < stream.Length)
            {
                var group = (SNOGroup)stream.ReadValueS32();
                var snoId = stream.ReadValueS32();
                var name = stream.ReadString(128, true);

                if (groupsToLoad != null && !groupsToLoad.Contains(group)) // if we're handled groups to load, just ignore the ones not in the list.
                    continue;

                var asset = new MPQAsset(group, snoId, name);
                asset.MpqFile = this.GetFile(asset.FileName, PatchExceptions.Contains(asset.Group)); // get the file. note: if file is in any of the groups in PatchExceptions it'll from load the original version - the reason is that assets in those groups got patched to 0 bytes. /raist.
                if (asset.MpqFile == null)
                    continue;

                // HACK: Quick workaround
                if (snoId == 19740)
                    continue;

                this.ProcessAsset(asset); // process the asset.

                if (++count % 1000 == 0)
                    Logger.Trace("Processed " + count + " assets");
            }

            stream.Close();

            // Run the parsers for assets (that have a parser).

            if (this._tasks.Count > 0) // if we're running in tasked mode, run the parser tasks.
            {
                foreach (var task in this._tasks)
                {
                    task.Start();
                }

                Task.WaitAll(this._tasks.ToArray()); // Wait all tasks to finish.
            }

            GC.Collect(); // force a garbage collection.
            GC.WaitForPendingFinalizers();

            var elapsedTime = DateTime.Now - timerStart;

            Logger.Info("Found a total of {0} assets from {1} catalog and parsed {2} of them in {3:c}.", assetsCount, fileName, parsedTasks, elapsedTime);
        }

        /// <summary>
        /// Adds the asset to the dictionary and tries to parse it if a parser
        /// is found and lazy loading is deactivated
        /// </summary>
        /// <param name="asset">New asset to be processed</param>
        private void ProcessAsset(Asset asset)
        {
            this.Assets[asset.Group].TryAdd(asset.SNOId, asset);
            if (!this.Parsers.ContainsKey(asset.Group)) return;

            asset.Parser = this.Parsers[asset.Group];

            try
            {
                asset.RunParser();
                parsedTasks++;
            }
            catch (Exception e)
            {
                Logger.Error("Error parsing {0}.\nMessage: {1}\n InnerException:{2}\nStack Trace:{3}", asset.FileName, e.Message, e.InnerException.Message, e.StackTrace);
            }
        }

        /// <summary>
        /// Gets a file from the mpq storage.
        /// </summary>
        /// <param name="fileName">File to read.</param>
        /// <param name="startSearchingFromBaseMPQ">Use the most available patched version? If you supply false to useMostAvailablePatchedVersion, it'll be looking for file starting from the base mpq up to latest available patch.</param>
        /// <returns>The MpqFile</returns>
        private MpqFile GetFile(string fileName, bool startSearchingFromBaseMPQ = false)
        {
            MpqFile file = null;

            if (fileName.Contains(".wrl") || fileName.Contains(".lvl"))
            {
                var i = 0;
                foreach (MpqArchive archive in this.FileSystem.Archives)
                {
                    if (i++ < 2) continue;

                    file = archive.FindFile(fileName);
                    if (file != null)
                        return file;
                }
            }

            if (!startSearchingFromBaseMPQ)
                file = this.FileSystem.FindFile(fileName);
            else
            {
                foreach (MpqArchive archive in this.FileSystem.Archives.Reverse()) //search mpqs starting from base
                {
                    file = archive.FindFile(fileName);
                    if (file != null)
                        break;
                }
            }

            return file;
        }
    }
}
