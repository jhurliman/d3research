using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Magic;
using libdiablo3.Process;

namespace libdiablo3.Api
{
    public class Diablo3Api : IDisposable
    {
        public static string PROCESS_NAME = "Diablo III";

        public event LogHandler OnLogMessage;

        private BlackMagic d3;
        private MemoryReader memReader;
        private Injector injector;
        private bool computedHash;

        public Diablo3Api()
        {
            Log.OnLogMessage += LogMessageCallback;

            d3 = new BlackMagic();
            d3.SetDebugPrivileges = false;
            memReader = new MemoryReader(d3);
        }

        public bool IsDiabloRunning()
        {
            return SProcess.GetProcessFromProcessName(PROCESS_NAME) != 0;
        }

        #region Init/Shutdown

        public bool Init()
        {
            // Open the Diablo III process
            int processID = SProcess.GetProcessFromProcessName(PROCESS_NAME);
            if (processID == 0)
            {
                Log.Error("Diablo III process not found");
                return false;
            }

            // Attempt to open the D3 process with read/write permission
            if (!d3.IsProcessOpen && !d3.Open(processID))
            {
                d3 = null;
                Log.Error("Failed to open Diablo III process " + processID);
                return false;
            }

            if (!computedHash)
            {
                // Compute the MD5 hash of the D3 executable to make sure we're working with the right version
                byte[] md5Bytes;
                using (FileStream exeStream = File.OpenRead(d3.GetModuleFilePath()))
                    md5Bytes = new MD5CryptoServiceProvider().ComputeHash(exeStream);
                string md5Hash = ProcessUtils.BytesToHexString(md5Bytes);
                if (md5Hash != Offsets.MD5_CLIENT)
                {
                    d3.Close();
                    d3 = null;
                    Log.Error("MD5 checksum failed: " + md5Hash);
                    return false;
                }

                computedHash = true;
            }

            // Initialize the memory reader, including important global pointer addresses
            if (!memReader.Init())
                return false;

            // Install our custom bytecode detour into the DirectX9 EndScene() method
            try
            {
                injector = new Injector(d3, memReader.EndSceneAddr);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to inject custom code", ex);
                return false;
            }

            return true;
        }

        public void Shutdown()
        {
            try
            {
                if (injector != null)
                    injector.Dispose();
                injector = null;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to dispose the injector", ex);
            }

            try
            {
                if (d3 != null)
                    d3.Close();
                d3 = null;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to close the D3 process", ex);
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        #endregion Init/Shutdown

        public World InitWorld()
        {
            if (memReader == null)
            {
                Log.Error("InitWorld() called before Init()");
                return null;
            }

            World world = new World();
            UpdateWorld(world);
            return world;
        }

        public void UpdateWorld(World world)
        {
            if (memReader == null)
            {
                Log.Error("UpdateWorld() called before Init()");
                return;
            }

            // FIXME: Finish all this
            List<D3Actor> d3Actors = memReader.GetActors();

            List<Gizmo> gizmos = new List<Gizmo>();
            List<Hero> heros = new List<Hero>();
            List<Monster> monsters = new List<Monster>();
            List<Item> items = new List<Item>();
            List<NPC> npcs = new List<NPC>();
            List<Actor> actors = new List<Actor>();

            for (int i = 0; i < d3Actors.Count; i++)
            {
                D3Actor d3Actor = d3Actors[i];

                Actor templateActor = ActorTemplates.Actors[d3Actor.SnoID];

                D3AttributeValue teamID;
                d3Actor.Attributes.TryGetValue(D3Attribute.TeamID, out teamID);

                Actor actor = new Actor(d3Actor.SnoID, (int)templateActor.Category, teamID.Value);
                actor.BoundingBox = new AABB(d3Actor.Pos1, d3Actor.Pos2);
                actor.Direction = d3Actor.Direction;
                actor.InstanceID = (int)d3Actor.ActorID;
                actors.Add(actor);
            }

            world.Me = null;
            world.Scenes = new Scene[0];

            world.Gizmos = new Gizmo[0];
            world.Heros = new Hero[0];
            world.Items = new Item[0];
            world.Monsters = new Monster[0];
            world.OtherActors = actors.ToArray();
        }

        private void LogMessageCallback(LogLevel level, string message, Exception ex)
        {
            LogHandler handler = OnLogMessage;
            if (handler != null)
                handler(level, message, ex);
        }
    }
}
