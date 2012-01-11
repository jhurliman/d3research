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
        public List<SceneAsset> Scenes;

        public MpqReader(string d3Path)
        {
            this.Scenes = new List<SceneAsset>();

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
                    if (group != SNOGroup.Scene)
                        continue;

                    var snoID = stream.ReadValueS32();
                    var name = stream.ReadString(128, true);
                    var filename = group + "\\" + name + ".scn";

                    MpqFile sceneFile = fs.FindFile(filename);
                    SceneAsset scene = new SceneAsset(sceneFile);
                    this.Scenes.Add(scene);
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
