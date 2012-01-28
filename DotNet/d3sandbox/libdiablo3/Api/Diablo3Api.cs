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
                    throw new MemoryReadException("MD5 checksum failed: " + md5Hash, 0);

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

            List<D3Actor> d3Actors;
            try { d3Actors = memReader.GetActors(); }
            catch (Exception ex) { Log.Warn("Failed to update actors: " + ex); return; }

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
                AABB aabb = new AABB(d3Actor.Pos1, d3Actor.Pos2);
                Vector2f direction = d3Actor.Direction;
                int instanceID = (int)d3Actor.ActorID;

                D3AttributeValue teamID;
                d3Actor.Attributes.TryGetValue(D3Attribute.TeamID, out teamID);

                if (templateActor is Gizmo)
                {
                    Gizmo gizmo = (Gizmo)templateActor;
                    gizmos.Add(Gizmo.CreateInstance(gizmo, instanceID, aabb, direction));
                }
                else if (templateActor is Hero)
                {
                    Hero hero = (Hero)templateActor;
                    heros.Add(Hero.CreateInstance(hero, instanceID, aabb, direction));
                }
                else if (templateActor is Item)
                {
                    Item item = (Item)templateActor;
                    items.Add(Item.CreateInstance(item, instanceID, aabb, direction));
                }
                else if (templateActor is Monster)
                {
                    if (d3Actor.Acd == null)
                        continue;

                    Monster monster = (Monster)templateActor;
                    int level = d3Actor.Attributes[D3Attribute.Level].Value;
                    int xpGranted = d3Actor.Attributes[D3Attribute.Experience_Granted].Value;
                    float hpCur = d3Actor.Attributes[D3Attribute.Hitpoints_Cur].ValueF;
                    float hpMax = d3Actor.Attributes[D3Attribute.Hitpoints_Max].Value;

                    monsters.Add(Monster.CreateInstance(monster, instanceID, aabb, direction, level,
                        xpGranted, hpCur, hpMax));
                }
                else if (templateActor is NPC)
                {
                    if (d3Actor.Acd == null)
                        continue;

                    NPC npc = (NPC)templateActor;
                    bool isOperatable = false; // FIXME:
                    int level = 0;
                    int hpCur = 0;
                    int hpMax = 0;
                    npcs.Add(NPC.CreateInstance(npc, instanceID, aabb, direction, isOperatable,
                        level, hpCur, hpMax));
                }
                else
                {
                    actors.Add(Actor.CreateInstance(templateActor, instanceID, aabb, direction));
                }
            }

            D3Actor d3Player = memReader.GetPlayer();
            memReader.GetAttribute(d3Player, D3Attribute.Gold);
            world.Me = null; // FIXME:

            world.Gizmos = gizmos.ToArray();
            world.Heros = heros.ToArray();
            world.Items = items.ToArray();
            world.Monsters = monsters.ToArray();
            world.NPCs = npcs.ToArray();
            world.OtherActors = actors.ToArray();

            #region Scenes

            List<D3Scene> d3Scenes;
            try { d3Scenes = memReader.GetScenes(); }
            catch (Exception ex) { Log.Warn("Failed to update scenes: " + ex); return; }

            world.Scenes = new Scene[d3Scenes.Count];
            for (int i = 0; i < d3Scenes.Count; i++)
            {
                D3Scene d3Scene = d3Scenes[i];
                NavCell[] navCells = NavCells.SceneNavCells[(int)d3Scene.SnoID];
                world.Scenes[i] = new Scene(d3Scene.Name, d3Scene.Active, d3Scene.SceneID,
                    d3Scene.WorldID, (int)d3Scene.SnoID, d3Scene.Position);
            }

            #endregion Scenes
        }

        private void LogMessageCallback(LogLevel level, string message, Exception ex)
        {
            LogHandler handler = OnLogMessage;
            if (handler != null)
                handler(level, message, ex);
        }
    }
}
