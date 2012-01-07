using System;
using System.Collections.Generic;
using System.Text;
using Magic;

namespace d3sandbox
{
    class Program
    {
        static uint GetPlayerGUID(BlackMagic d3, uint pObjMgr)
        {
            const uint INVALID = 0xFFFFFFFF;

            //v0 = *(gpObjMgr + 2352);
            //if (*v0 != -1 && (v1 = *(getObjMgrPlus1916() + 168)) != 0 && (v2 = 30520 * *v0 + v1 + 88) != 0)
            //    result = *(v2 + 8);

            uint v0 = d3.ReadUInt(d3.ReadUInt(pObjMgr + 2352));
            if (v0 == INVALID)
                return INVALID;

            uint v1 = d3.ReadUInt(pObjMgr + 1916 + 168);
            if (v1 == 0)
                return INVALID;

            uint v2 = 30520 * v0 + v1 + 88;
            if (v2 == 0)
                return INVALID;

            return d3.ReadUInt(v2 + 8);
        }

        static RActor GetRActorFromGUID(BlackMagic d3, uint pRActors, uint guid)
        {
            uint v0 = d3.ReadUInt(pRActors + 328);
            int v1 = d3.ReadInt(pRActors + 396);

            uint ptr = d3.ReadUInt(v0 + 4 * ((guid & 0xFFFF) >> v1)) + 1068 * (uint)(guid & ((1 << v1) - 1));
            if (d3.ReadUInt(ptr) == guid)
                return GetRActor(d3, ptr);
            return null;
        }

        static RActor GetRActor(BlackMagic d3, uint ptr)
        {
            if (d3.ReadUInt(ptr) == 0xFFFFFFFF)
                return null;

            return new RActor(d3.ReadBytes(ptr, 1068));
        }

        static List<RActor> GetRActors(BlackMagic d3)
        {
            // Fetch the global pointer to the object manager
            uint pObjMgr = d3.ReadUInt(0x140593C);

            // Fetch the the global pointer to the RActors container
            uint pRActors = d3.ReadUInt(pObjMgr + 0x8AC);
            if (d3.ReadASCIIString(pRActors, 7) != "RActors")
                throw new InvalidHostException("Cannot find RActors");

            // Grab the size of the RActors array
            int actorArraySize = d3.ReadInt(pRActors + 268);

            // Get the player actor
            uint playerGuid = GetPlayerGUID(d3, pObjMgr);
            RActor playerActor = GetRActorFromGUID(d3, pRActors, playerGuid);
            Console.WriteLine("Player actor: " + playerActor);

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
