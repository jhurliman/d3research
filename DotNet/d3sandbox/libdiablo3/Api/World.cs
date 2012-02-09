using System;
using System.Collections;
using System.Collections.Generic;
using libdiablo3.Process;

namespace libdiablo3.Api
{
    public class World
    {
        private MemoryReader memReader;
        private Injector injector;
        private WorldActorEnumerator enumerator;
        private uint scenesCRC;

        public Player Me { get; internal set; }
        public Scene[] Scenes { get; internal set; }
        public Hero[] Heros { get; internal set; }
        public Monster[] Monsters { get; internal set; }
        public NPC[] NPCs { get; internal set; }
        public Item[] Items { get; internal set; }
        public Gizmo[] Gizmos { get; internal set; }
        public Actor[] OtherActors { get; internal set; }
        public byte[,] WalkableGrid { get; internal set; }
        public AABB BoundingBox { get; internal set; }
        public uint ScenesCRC { get { return scenesCRC; } }
        public IEnumerable<Actor> AllActors { get { return enumerator; } }

        internal World(MemoryReader memReader, Injector injector)
        {
            this.memReader = memReader;
            this.injector = injector;
            this.enumerator = new WorldActorEnumerator(this);
        }

        public Actor GetClosestWaypoint()
        {
            // FIXME:
            return null;
        }

        public Monster GetClosestMonster()
        {
            // FIXME:
            return null;
        }

        internal void Update()
        {
            UpdatePlayer();
            UpdateActors();
            UpdateScenes();
        }

        private void UpdatePlayer()
        {
            D3Actor d3Player = memReader.GetPlayer();
            if (d3Player != null)
            {
                this.Me = Player.CreateInstance(this.injector, d3Player.Pointer, d3Player.Acd.Pointer,
                    d3Player.SnoID, (int)d3Player.ActorID, (int)d3Player.AcdID,
                    new AABB(d3Player.Pos1, d3Player.Pos2), d3Player.Direction, d3Player.WorldID);

                this.Me.SkillSlots.UpdateSkills(memReader.GetActiveSkills());

                this.Me.Backpack = new Backpack(d3Player.Acd.Attributes[D3Attribute.Backpack_Slots].Value);
                this.Me.Stash = new Stash(d3Player.Acd.Attributes[D3Attribute.Shared_Stash_Slots].Value);

                this.Me.Health = new Resource(
                    d3Player.Acd.Attributes[D3Attribute.Hitpoints_Cur].Value,
                    d3Player.Acd.Attributes[D3Attribute.Hitpoints_Max].Value);

                D3AttributeValue primaryResource = d3Player.Acd.Attributes[D3Attribute.Resource_Type_Primary];
                this.Me.PrimaryResourceType = (ResourceType)primaryResource.Value;
                this.Me.PrimaryResource = new Resource(
                    (int)d3Player.Acd.Attributes[D3Attribute.Resource_Cur, primaryResource.Value].ValueF,
                    (int)d3Player.Acd.Attributes[D3Attribute.Resource_Max, primaryResource.Value].ValueF);

                D3AttributeValue secondaryResource;
                if (d3Player.Acd.Attributes.TryGetValue(D3Attribute.Resource_Type_Secondary, out secondaryResource))
                    this.Me.SecondaryResourceType = (ResourceType)secondaryResource.Value;
                else
                    this.Me.SecondaryResourceType = ResourceType.None;
                if (this.Me.SecondaryResourceType != ResourceType.None)
                {
                    this.Me.SecondaryResource = new Resource(
                        (int)d3Player.Acd.Attributes[D3Attribute.Resource_Cur, secondaryResource.Value].ValueF,
                        (int)d3Player.Acd.Attributes[D3Attribute.Resource_Max, secondaryResource.Value].ValueF);
                }

                this.Me.Level = d3Player.Acd.Attributes[D3Attribute.Level].Value;
                this.Me.XPNextLevel = Experience.Levels[this.Me.Level];
                this.Me.XP = this.Me.XPNextLevel - d3Player.Acd.Attributes[D3Attribute.Experience_Next].Value;

                this.Me.GlobalCooldown = d3Player.Acd.Attributes[D3Attribute.General_Cooldown].Value;
            }
            else
            {
                this.Me = null;
            }
        }

        private void UpdateActors()
        {
            Dictionary<int, D3ActorCommonData> d3ACDs = memReader.GetACDs();

            List<Hero> heros = new List<Hero>();
            List<Gizmo> gizmos = new List<Gizmo>();
            List<Monster> monsters = new List<Monster>();
            List<Item> items = new List<Item>();
            Dictionary<int, NPC> npcs = new Dictionary<int, NPC>();
            List<Actor> actors = new List<Actor>();

            foreach (D3ActorCommonData d3ACD in d3ACDs.Values)
            {
                // Handle inventory and equipped items
                if (d3ACD.Placement != -1)
                {
                    PlaceItem(d3ACD);
                    continue;
                }

                Actor templateActor = ActorTemplates.Actors[d3ACD.SnoID];
                AABB aabb = new AABB(d3ACD.Pos1, d3ACD.Pos2);
                Vector2f direction = d3ACD.Direction;
                int acdID = (int)d3ACD.AcdID;

                D3AttributeValue teamID;
                d3ACD.Attributes.TryGetValue(D3Attribute.TeamID, out teamID);

                if (templateActor is Gizmo)
                {
                    Gizmo gizmo = Gizmo.CreateInstance((Gizmo)templateActor, acdID, acdID, aabb, direction);
                    gizmos.Add(gizmo);
                }
                else if (templateActor is Hero)
                {
                    if (this.Me == null || acdID != this.Me.AcdID)
                    {
                        Hero hero = Hero.CreateInstance((Hero)templateActor, acdID, acdID, aabb, direction);
                        heros.Add(hero);
                    }
                }
                else if (templateActor is Item)
                {
                    int itemTypeHash;
                    if (!ItemDefinitions.Definitions.TryGetValue((int)d3ACD.GBID, out itemTypeHash))
                        throw new MemoryReadException("Unrecognized GBID", d3ACD.GBID);

                    ItemType type = ItemTypes.Types[itemTypeHash];
                    Item item = Item.CreateInstance((Item)templateActor, acdID, acdID, aabb, direction, type, -1, 0, 0);
                    items.Add(item);
                }
                else if (templateActor is Monster)
                {
                    int level = d3ACD.Attributes[D3Attribute.Level].Value;
                    int xpGranted = d3ACD.Attributes[D3Attribute.Experience_Granted].Value;
                    float hpCur = d3ACD.Attributes[D3Attribute.Hitpoints_Cur].ValueF;
                    float hpMax = d3ACD.Attributes[D3Attribute.Hitpoints_Max].Value;

                    Monster monster = Monster.CreateInstance((Monster)templateActor, acdID, acdID, aabb, direction, level,
                        xpGranted, hpCur, hpMax);
                    monsters.Add(monster);
                }
                else if (templateActor is NPC)
                {
                    bool isOperatable = false; // FIXME:
                    int level = 0;
                    int hpCur = 0;
                    int hpMax = 0;

                    NPC npc = NPC.CreateInstance((NPC)templateActor, acdID, acdID, aabb, direction, isOperatable,
                        level, hpCur, hpMax);
                    npcs.Add(npc.InstanceID, npc);
                }
                else
                {
                    Actor actor = Actor.CreateInstance(templateActor, acdID, acdID, aabb, direction);
                    actors.Add(actor);
                }
            }

            this.Heros = heros.ToArray();
            this.Gizmos = gizmos.ToArray();
            this.Items = items.ToArray();
            this.Monsters = monsters.ToArray();
            if (this.NPCs == null || this.NPCs.Length != npcs.Count)
                this.NPCs = new NPC[npcs.Count];
            npcs.Values.CopyTo(this.NPCs, 0);
            this.OtherActors = actors.ToArray();
        }

        private void UpdateScenes()
        {
            List<D3Scene> d3Scenes = memReader.GetScenes();
            
            // Check if the current scene list changed since last time
            uint curCRC = 0;
            for (int i = 0; i < d3Scenes.Count; i++)
                curCRC ^= d3Scenes[i].SceneID;
            if (this.scenesCRC == curCRC)
                return;
            this.scenesCRC = curCRC;

            Scene[] scenes = new Scene[d3Scenes.Count];
            for (int i = 0; i < d3Scenes.Count; i++)
            {
                D3Scene d3Scene = d3Scenes[i];
                NavCell[] navCells = NavCells.SceneNavCells[(int)d3Scene.SnoID];
                scenes[i] = new Scene(d3Scene.Name, d3Scene.Active, d3Scene.SceneID,
                    d3Scene.WorldID, (int)d3Scene.SnoID, d3Scene.Position);
            }

            // Find the world bounding box
            AABB boundingBox = new AABB(
                new Vector3f(Single.MaxValue, Single.MaxValue, 0f),
                new Vector3f(Single.MinValue, Single.MinValue, 0f));
            for (int i = 0; i < scenes.Length; i++)
            {
                AABB aabb = scenes[i].BoundingBox;
                if (aabb.Min.X < BoundingBox.Min.X) boundingBox.Min.X = aabb.Min.X;
                if (aabb.Min.Y < boundingBox.Min.Y) boundingBox.Min.Y = aabb.Min.Y;
                if (aabb.Max.X > boundingBox.Max.X) boundingBox.Max.X = aabb.Max.X;
                if (aabb.Max.Y > boundingBox.Max.Y) boundingBox.Max.Y = aabb.Max.Y;
            }
            this.BoundingBox = boundingBox;

            // Create the walkable grid
            int width = NextPowerOfTwo((int)boundingBox.Max.X / 4);
            int height = NextPowerOfTwo((int)boundingBox.Max.Y / 4);
            byte[,] walkableGrid = new byte[width, height];

            for (int i = 0; i < scenes.Length; i++)
            {
                Scene scene = scenes[i];

                for (int j = 0; j < scene.NavCells.Length; j++)
                {
                    NavCell cell = scene.NavCells[j];
                    if (!cell.Flags.HasFlag(NavCellFlags.AllowWalk))
                        continue;

                    Vector2i min = new Vector2i(
                        (int)((scene.BoundingBox.Min.X + cell.BoundingBox.Min.X) * 0.25f),
                        (int)((scene.BoundingBox.Min.Y + cell.BoundingBox.Min.Y) * 0.25f));
                    Vector2i max = new Vector2i(
                        (int)((scene.BoundingBox.Min.X + cell.BoundingBox.Max.X) * 0.25f),
                        (int)((scene.BoundingBox.Min.Y + cell.BoundingBox.Max.Y) * 0.25f));

                    for (int x = min.X; x <= max.X; x++)
                    {
                        for (int y = min.Y; y <= max.Y; y++)
                            walkableGrid[x, y] = 1;
                    }
                }
            }

            this.Scenes = scenes;
            this.WalkableGrid = walkableGrid;
        }

        private void PlaceItem(D3ActorCommonData d3ACD)
        {
            switch ((ItemPlacement)d3ACD.Placement)
            {
                case ItemPlacement.Merchant:
                    throw new NotImplementedException("Merchant items");
                /*NPC merchant;
                if (npcs.TryGetValue(d3ACD.OwnerID, out merchant))
                {
                    if (merchant.Inventory == null)
                        merchant.Inventory = new Inventory(10, 60);
                    merchant.Inventory.AddItem(CreateItem(d3ACD));
                }
                break;*/
                case ItemPlacement.PlayerBackpack:
                    this.Me.Backpack.AddItem(CreateItem(d3ACD));
                    break;
                case ItemPlacement.PlayerStash:
                    this.Me.Stash.AddItem(CreateItem(d3ACD));
                    break;
                case ItemPlacement.PlayerGold:
                    if (d3ACD.GBID != Offsets.GOLD_GBID)
                    {
                        Log.Warn("Unrecognized item " + d3ACD.ModelName + " in the gold slot");
                        break;
                    }
                    this.Me.Gold = d3ACD.Attributes[D3Attribute.ItemStackQuantityLo].Value;
                    break;
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
                    this.Me.Outfit.AddItem((ItemPlacement)d3ACD.Placement, CreateItem(d3ACD));
                    break;
                case ItemPlacement.Unknown1:
                case ItemPlacement.Unknown2:
                case ItemPlacement.Unknown3:
                case ItemPlacement.Unknown4:
                case ItemPlacement.Unknown5:
                default:
                    throw new MemoryReadException("Unknown item placement " + d3ACD.Placement +
                        " for " + d3ACD.ModelName, d3ACD.Pointer);
            }
        }

        private Item CreateItem(D3ActorCommonData d3ACD)
        {
            Actor templateActor = ActorTemplates.Actors[d3ACD.SnoID];
            ItemType type = ItemTypes.Types[ItemDefinitions.Definitions[(int)d3ACD.GBID]];
            Item item = Item.CreateInstance((Item)templateActor, -1, d3ACD.AcdID, AABB.Zero,
                Vector2f.Zero, type, d3ACD.Placement, d3ACD.InventoryX,
                d3ACD.InventoryY);
            item.InstanceID = d3ACD.AcdID;
            return item;
        }

        #region Enumerator

        public class WorldActorEnumerator : IEnumerable<Actor>
        {
            private World world;

            public WorldActorEnumerator(World world)
            {
                this.world = world;
            }

            public IEnumerator<Actor> GetEnumerator()
            {
                if (world.Me != null)
                    yield return world.Me;

                if (world.Heros == null)
                    yield break;

                foreach (Hero hero in world.Heros)
                    yield return hero;
                foreach (Monster monster in world.Monsters)
                    yield return monster;
                foreach (Item item in world.Items)
                    yield return item;
                foreach (Gizmo gizmo in world.Gizmos)
                    yield return gizmo;
                foreach (Actor actor in world.OtherActors)
                    yield return actor;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion Enumerator

        private static int NextPowerOfTwo(int k)
        {
            --k;
            for (int i = 1; i < 32; i <<= 1)
                k |= k >> i;
            return k + 1;
        }
    }
}
