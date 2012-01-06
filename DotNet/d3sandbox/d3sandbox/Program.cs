using System;
using System.Collections.Generic;
using System.Text;
using Magic;

namespace d3sandbox
{
    class Program
    {
        static RActor GetRActor(BlackMagic d3, uint ptr)
        {
            if (d3.ReadUInt(ptr) == 0xFFFFFFFF)
                return null;

            return new RActor(d3.ReadBytes(ptr, 1068));
        }

        static List<RActor> GetRActors(BlackMagic d3)
        {
            // Fetch the the global pointer to the RActors container
            uint pRActors = d3.ReadUInt(d3.ReadUInt(0x140593C) + 0x8AC);
            if (d3.ReadASCIIString(pRActors, 7) != "RActors")
                throw new InvalidHostException("Cannot find RActors");

            // Grab the size of the RActors array
            int actorArraySize = d3.ReadInt(pRActors + 268);

            // Grab the first RActor
            uint pRActor = d3.ReadUInt(d3.ReadUInt(pRActors + 328));
            
            // Loop through the array and grab all valid actor objects
            List<RActor> actors = new List<RActor>(actorArraySize);
            for (uint i = 0; i < actorArraySize; i++)
            {
                RActor actor = GetRActor(d3, pRActor + i * 1068);
                if (actor != null)
                    actors.Add(actor);
            }

            return actors;
        }

        static void Main(string[] args)
        {
            BlackMagic d3 = new BlackMagic();
            if (d3.OpenProcessAndThread(SProcess.GetProcessFromProcessName("Diablo III")))
            {
                // TODO: Check md5sum of the executable
                Console.WriteLine(d3.GetModuleFilePath());

                // Enable a debug flag in the client. Valid values seem to be 0-23, although 8 crashes v8101
                d3.WriteInt(0x1405978, 20);

                List<RActor> actors = GetRActors(d3);

                Console.WriteLine("RActors ({0} total):", actors.Count);
                foreach (RActor actor in actors)
                    Console.WriteLine(actor);
            }
        }
    }
}
