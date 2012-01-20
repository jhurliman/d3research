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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CrystalMpq;
using Mooege.Common.Logging;

namespace Mooege.Common.MPQ
{    
    public class MPQPatchChain
    {
        protected static readonly Logger Logger = LogManager.CreateLogger();

        public bool Loaded { get; private set; }
        public List<string> BaseMPQFiles = new List<string>();
        public string PatchPattern { get; private set; }
        public readonly SortedList<int, string> MPQFileList = new SortedList<int, string>();
        public readonly MpqFileSystem FileSystem = new MpqFileSystem();

        protected MPQPatchChain(IEnumerable<string> baseFiles, string patchPattern=null)
        {
            this.Loaded = false;

            foreach(var file in baseFiles)
            {
                var mpqFile = MPQStorage.GetMPQFile(file);
                if(mpqFile == null)
                {
                    Logger.Error("Cannot find base MPQ file: {0}.", file);
                    return;
                }
                this.BaseMPQFiles.Add(mpqFile);
                Logger.Trace("Added base-mpq file: {0}.", file);
            }
                        
            this.PatchPattern = patchPattern;
            this.ConstructChain();

            var topMostMPQVersion = this.MPQFileList.Reverse().First().Key; // check required version.
            this.Loaded = true;        
        }

        private void ConstructChain()
        {            
            // add base mpq files;
            for (int i = 0; i < this.BaseMPQFiles.Count; i++)
                MPQFileList.Add(i, this.BaseMPQFiles[i]);

            if (PatchPattern == null) return;

            /* match the mpq files for the patch chain */
            var patchRegex = new Regex(this.PatchPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach(var file in MPQStorage.MPQList)
            {
                var match = patchRegex.Match(file);
                if (!match.Success) continue;
                if (!match.Groups["version"].Success) continue;

                var patchName = match.Groups[0].Value;
                var patchVersion = Int32.Parse(match.Groups["version"].Value);

                MPQFileList.Add(patchVersion, file);
                Logger.Trace("Added patch file: {0}.", patchName);
            }

            /* add mpq's to mpq-file system in reverse-order (highest version first) */
            foreach(var pair in this.MPQFileList.Reverse())
            {
                string mpq = pair.Value;
                Logger.Trace("Applying file: {0}.", System.IO.Path.GetFileName(mpq));
                this.FileSystem.Archives.Add(new MpqArchive(mpq, true));
            }
            Logger.Trace("All files successfully applied.");
        }
    }
}
