using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Magic;

namespace d3sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            BlackMagic d3 = new BlackMagic();
            if (d3.OpenProcessAndThread(SProcess.GetProcessFromProcessName("Diablo III")))
            {
                // TODO: Check md5sum of the executable
                string exePath = d3.GetModuleFilePath();
                Console.WriteLine(exePath);

                //MpqReader mpqReader = new MpqReader(Path.GetDirectoryName(exePath));
                //Console.WriteLine("Loaded " + mpqReader.Scenes.Count + " scenes from MPQ archives");

                // Enable a debug flag in the client. Valid values seem to be 0-23, although 8 crashes v8101
                d3.WriteInt(0x1405978, 20);

                MemoryReader memReader = new MemoryReader(d3);

                // Fetch the current player
                Player player = memReader.GetPlayer();
                Console.WriteLine("Player: " + player);

                // Fetch all entities
                List<Entity> entities = memReader.GetEntities();
                Console.WriteLine();
                Console.WriteLine("Entities ({0} total):", entities.Count);
                foreach (Entity entity in entities)
                    Console.WriteLine(entity);

                // Fetch all scenes
                List<Scene> scenes = memReader.GetScenes();
                Console.WriteLine();
                Console.WriteLine("Scenes ({0} total):", scenes.Count);
                foreach (Scene scene in scenes)
                    Console.WriteLine(scene);
            }
        }
    }
}
