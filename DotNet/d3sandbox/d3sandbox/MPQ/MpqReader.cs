using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CrystalMpq;
using Gibbed.IO;

namespace d3sandbox
{
    public class MpqReader
    {
        public Dictionary<int, SceneAsset> Scenes;

        public MpqReader(string d3Path)
        {
            this.Scenes = new Dictionary<int, SceneAsset>();

            List<string> mpqFiles = GetFilesByExtensionRecursive(Path.Combine(d3Path, "Data_D3\\PC\\MPQs"), ".mpq");
            SortedList<int, string> mpqFileList = new SortedList<int, string>();
            
            // Add the base CoreData.mpq
            string coreFile = mpqFiles.FirstOrDefault(file => file.Contains("CoreData.mpq"));
            mpqFileList.Add(0, coreFile);

            // Add the patch files
            var patchRegex = new Regex("/base/d3-update-base-(?<version>.*?).mpq", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (var file in mpqFiles)
            {
                var match = patchRegex.Match(file);
                if (!match.Success) continue;
                if (!match.Groups["version"].Success) continue;

                var patchName = match.Groups[0].Value;
                var patchVersion = Int32.Parse(match.Groups["version"].Value);

                mpqFileList.Add(patchVersion, file);
            }

            // Add MPQs to MPQ filesystem in reverse-order (highest version first)
            MpqFileSystem fs = new MpqFileSystem();
            foreach (var pair in mpqFileList.Reverse())
            {
                var mpq = pair.Value;
                Console.WriteLine("Applying MPQ file: {0}", Path.GetFileName(mpq));
                fs.Archives.Add(new MpqArchive(mpq, true));
            }

            // Load the catalog file
            MpqFile toc = fs.FindFile("CoreTOC.dat");
            using (var stream = toc.Open())
            {
                var assetsCount = stream.ReadValueS32();

                while (stream.Position < stream.Length)
                {
                    var group = (SNOGroup)stream.ReadValueS32();
                    var snoID = stream.ReadValueS32();
                    var name = stream.ReadString(128, true);

                    if (group == SNOGroup.Scene)
                    {
                        var filename = group + "\\" + name + ".scn";
                        MpqFile sceneFile = fs.FindFile(filename);
                        SceneAsset scene = new SceneAsset(sceneFile);
                        this.Scenes[snoID] = scene;
                    }
                    else if (group == SNOGroup.Power)
                    {
                        //Console.WriteLine("Power: {0} ({1:X})", name, snoID);
                    }
                }
            }
        }

        private static List<string> GetFilesByExtensionRecursive(string directory, string fileExtension)
        {
            var files = new List<string>(); // Store results in the file results list.
            var stack = new Stack<string>(); // Store a stack of our directories.

            stack.Push(directory); // Add initial directory.

            while (stack.Count > 0) // Continue while there are directories to process
            {
                var topDir = stack.Pop(); // Get top directory
                var dirInfo = new DirectoryInfo(topDir);

                files.AddRange((from fileInfo in dirInfo.GetFiles()
                                where string.Compare(fileInfo.Extension, fileExtension, System.StringComparison.OrdinalIgnoreCase) == 0
                                select topDir + "/" + fileInfo.Name).ToList());

                foreach (var dir in Directory.GetDirectories(topDir)) // Add all directories at this directory.
                {
                    stack.Push(dir);
                }
            }

            return files.Select(file => file.Replace("\\", "/")).ToList();
        }
    }
}
