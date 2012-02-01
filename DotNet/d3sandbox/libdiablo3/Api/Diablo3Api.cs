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
        public const string PROCESS_NAME = "Diablo III";
        private const uint GOLD_GBID = 0x07869277;

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
            if (!memReader.UpdatePointers())
                return false;

            // Install our detour into the DirectX9 EndScene() method
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

            if (!memReader.UpdatePointers())
            {
                Log.Warn("Failed to update pointers");
                return;
            }

            List<D3Scene> d3Scenes;
            Dictionary<int, D3Actor> d3Actors;
            Dictionary<int, D3ActorCommonData> d3ACDs;
            try
            {
                d3Scenes = memReader.GetScenes();
                d3Actors = memReader.GetActors();
                d3ACDs = memReader.GetACDs();
            }
            catch (Exception ex) { Log.Warn("Failed to fetch scene info: " + ex); return; }

            #region Scenes

            world.Scenes = new Scene[d3Scenes.Count];
            for (int i = 0; i < d3Scenes.Count; i++)
            {
                D3Scene d3Scene = d3Scenes[i];
                NavCell[] navCells = NavCells.SceneNavCells[(int)d3Scene.SnoID];
                world.Scenes[i] = new Scene(d3Scene.Name, d3Scene.Active, d3Scene.SceneID,
                    d3Scene.WorldID, (int)d3Scene.SnoID, d3Scene.Position);
            }

            #endregion Scenes

            #region Actors

            List<Hero> heros = new List<Hero>();
            List<Gizmo> gizmos = new List<Gizmo>();
            List<Monster> monsters = new List<Monster>();
            List<Item> items = new List<Item>();
            Dictionary<int, NPC> npcs = new Dictionary<int, NPC>();
            List<Actor> actors = new List<Actor>();

            foreach (D3Actor d3Actor in d3Actors.Values)
            {
                Actor templateActor = ActorTemplates.Actors[d3Actor.SnoID];
                AABB aabb = new AABB(d3Actor.Pos1, d3Actor.Pos2);
                Vector2f direction = d3Actor.Direction;
                int instanceID = (int)d3Actor.ActorID;

                D3AttributeValue teamID;
                d3Actor.Attributes.TryGetValue(D3Attribute.TeamID, out teamID);

                if (templateActor is Gizmo)
                {
                    Gizmo gizmo = Gizmo.CreateInstance((Gizmo)templateActor, instanceID, aabb, direction);
                    gizmos.Add(gizmo);
                }
                else if (templateActor is Hero)
                {
                    Hero hero = Hero.CreateInstance((Hero)templateActor, instanceID, aabb, direction);
                    heros.Add(hero);
                }
                else if (templateActor is Item)
                {
                    int itemTypeHash;
                    if (!ItemDefinitions.SnoToDefinitions.TryGetValue(d3Actor.SnoID, out itemTypeHash))
                        continue;

                    ItemType type = ItemTypes.Types[itemTypeHash];
                    Item item = Item.CreateInstance((Item)templateActor, instanceID, aabb, direction, type, -1, 0, 0);
                    items.Add(item);
                }
                else if (templateActor is Monster)
                {
                    if (d3Actor.Acd == null)
                        continue;

                    int level = d3Actor.Attributes[D3Attribute.Level].Value;
                    int xpGranted = d3Actor.Attributes[D3Attribute.Experience_Granted].Value;
                    float hpCur = d3Actor.Attributes[D3Attribute.Hitpoints_Cur].ValueF;
                    float hpMax = d3Actor.Attributes[D3Attribute.Hitpoints_Max].Value;

                    Monster monster = Monster.CreateInstance((Monster)templateActor, instanceID, aabb, direction, level,
                        xpGranted, hpCur, hpMax);
                    monsters.Add(monster);
                }
                else if (templateActor is NPC)
                {
                    if (d3Actor.Acd == null)
                        continue;

                    bool isOperatable = false; // FIXME:
                    int level = 0;
                    int hpCur = 0;
                    int hpMax = 0;

                    NPC npc = NPC.CreateInstance((NPC)templateActor, instanceID, aabb, direction, isOperatable,
                        level, hpCur, hpMax);
                    npcs.Add(npc.InstanceID, npc);
                }
                else
                {
                    Actor actor = Actor.CreateInstance(templateActor, instanceID, aabb, direction);
                    actors.Add(actor);
                }
            }

            #endregion Actors

            #region Player

            D3Actor d3Player = memReader.GetPlayer();
            if (d3Player != null)
            {
                world.Me = new Player(d3Player.SnoID);
                world.Me.Backpack = new Backpack(d3Player.Attributes[D3Attribute.Backpack_Slots].Value);
                world.Me.Stash = new Stash(d3Player.Attributes[D3Attribute.Shared_Stash_Slots].Value);
                world.Me.SkillSlots.UpdateSkills(memReader.GetActiveSkills());
            }
            else
            {
                world.Me = null;
            }

            #endregion Player

            #region Inventories

            foreach (D3ActorCommonData d3ACD in d3ACDs.Values)
            {
                switch ((ItemPlacement)d3ACD.Placement)
                {
                    case ItemPlacement.Merchant:
                    {
                        throw new NotImplementedException("Merchant items");

                        NPC merchant;
                        if (npcs.TryGetValue(d3ACD.OwnerID, out merchant))
                        {
                            if (merchant.Inventory == null)
                                merchant.Inventory = new Inventory(10, 60);
                            merchant.Inventory.AddItem(CreateItem(d3ACD));
                        }
                        break;
                    }
                    case ItemPlacement.PlayerBackpack:
                    {
                        world.Me.Backpack.AddItem(CreateItem(d3ACD));
                        break;
                    }
                    case ItemPlacement.PlayerStash:
                    {
                        world.Me.Stash.AddItem(CreateItem(d3ACD));
                        break;
                    }
                    case ItemPlacement.PlayerGold:
                    {
                        if (d3ACD.GBID != GOLD_GBID)
                        {
                            Log.Warn("Unrecognized item " + d3ACD.ModelName + " in the gold slot");
                            break;
                        }
                        world.Me.Gold = memReader.GetAttribute(d3ACD.AttributesPtr,
                            D3Attribute.ItemStackQuantityLo).Value.Value;
                        break;
                    }
                    case ItemPlacement.PlayerBracers:
                    case ItemPlacement.PlayerFeet:
                    case ItemPlacement.PlayerHands:
                    case ItemPlacement.PlayerHead:
                    case ItemPlacement.PlayerLeftFinger:
                    case ItemPlacement.PlayerLeftHand:
                    case ItemPlacement.PlayerLegs:
                    case ItemPlacement.PlayerNeck:
                    case ItemPlacement.PlayerRightFinger:
                    case ItemPlacement.PlayerRightHand:
                    case ItemPlacement.PlayerShoulders:
                    case ItemPlacement.PlayerTorso:
                    case ItemPlacement.PlayerWaist:
                    {
                        world.Me.Outfit.AddItem((ItemPlacement)d3ACD.Placement, CreateItem(d3ACD));
                        break;
                    }
                    case ItemPlacement.Unknown1:
                    case ItemPlacement.Unknown2:
                    case ItemPlacement.Unknown3:
                    case ItemPlacement.Unknown4:
                    case ItemPlacement.Unknown5:
                    {
                        Log.Warn("Unknown item placement " + d3ACD.Placement + " for " + d3ACD.ModelName);
                        break;
                    }
                }
            }

            #endregion Inventories

            world.Heros = heros.ToArray();
            world.Gizmos = gizmos.ToArray();
            world.Items = items.ToArray();
            world.Monsters = monsters.ToArray();
            if (world.NPCs == null || world.NPCs.Length != npcs.Count)
                world.NPCs = new NPC[npcs.Count];
            npcs.Values.CopyTo(world.NPCs, 0);
            world.OtherActors = actors.ToArray();
        }

        private Item CreateItem(D3ActorCommonData d3ACD)
        {
            Actor templateActor = ActorTemplates.Actors[d3ACD.SnoID];
            ItemType type = ItemTypes.Types[ItemDefinitions.Definitions[(int)d3ACD.GBID]];
            Item item = Item.CreateInstance((Item)templateActor, -1, AABB.Zero,
                Vector2f.Zero, type, d3ACD.Placement, d3ACD.InventoryX,
                d3ACD.InventoryY);
            item.InstanceID = d3ACD.AcdID;
            return item;
        }

        private void LogMessageCallback(LogLevel level, string message, Exception ex)
        {
            LogHandler handler = OnLogMessage;
            if (handler != null)
                handler(level, message, ex);
        }
    }
}
