using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class Backpack : Inventory
    {
        public Backpack(int slotCount)
            : base(10, slotCount)
        {
        }
    }

    public class Stash : Inventory
    {
        public Stash(int slotCount)
            : base(7, slotCount)
        {
        }
    }

    public struct Resource
    {
        public readonly int Value;
        public readonly int Max;

        public Resource(int value, int max)
        {
            Value = value;
            Max = max;
        }
    }

    public class Player : Hero
    {
        #region Classes

        public class PlayerOutfit
        {
            public Item Bracers { get; internal set; }
            public Item Feet { get; internal set; }
            public Item Hands { get; internal set; }
            public Item Head { get; internal set; }
            public Item LeftFinger { get; internal set; }
            public Item LeftHand { get; internal set; }
            public Item Legs { get; internal set; }
            public Item Neck { get; internal set; }
            public Item RightFinger { get; internal set; }
            public Item RightHand { get; internal set; }
            public Item Shoulders { get; internal set; }
            public Item Torso { get; internal set; }
            public Item Waist { get; internal set; }

            internal void AddItem(ItemPlacement placement, Item item)
            {
                switch (placement)
                {
                    case ItemPlacement.PlayerBracers: Bracers = item; break;
                    case ItemPlacement.PlayerFeet: Feet = item; break;
                    case ItemPlacement.PlayerHands: Hands = item; break;
                    case ItemPlacement.PlayerHead: Head = item; break;
                    case ItemPlacement.PlayerLeftFinger: LeftFinger = item; break;
                    case ItemPlacement.PlayerLeftHand: LeftHand = item; break;
                    case ItemPlacement.PlayerLegs: Legs = item; break;
                    case ItemPlacement.PlayerNeck: Neck = item; break;
                    case ItemPlacement.PlayerRightFinger: RightFinger = item; break;
                    case ItemPlacement.PlayerRightHand: RightHand = item; break;
                    case ItemPlacement.PlayerShoulders: Shoulders = item; break;
                    case ItemPlacement.PlayerTorso: Torso = item; break;
                    case ItemPlacement.PlayerWaist: Waist = item; break;
                    default:
                        throw new Exception("Unrecognized attachment position " + placement);
                }
            }
        }

        #endregion Classes

        public readonly SkillSlots SkillSlots;
        public readonly PlayerOutfit Outfit;
        
        public Backpack Backpack { get; internal set; }
        public Stash Stash { get; internal set; }
        public int Gold { get; internal set; }
        public float GlobalCooldown { get; internal set; }

        public Resource Health { get; internal set; }
        public Resource Fury { get; internal set; }
        public Resource Spirit { get; internal set; }
        public Resource Mana { get; internal set; }
        public Resource ArcanePower { get; internal set; }
        public Resource Hatred { get; internal set; }
        public Resource Discipline { get; internal set; }

        public Player(int snoID)
            : base(snoID)
        {
            SkillSlots = new SkillSlots();
            Outfit = new PlayerOutfit();
        }

        internal static Player CreateInstance(int snoID, int instanceID, int acdID, AABB aabb,
            Vector2f direction)
        {
            Player player = new Player(snoID);
            player.InstanceID = instanceID;
            player.AcdID = acdID;
            player.BoundingBox = aabb;
            player.Direction = direction;
            return player;
        }
    }
}
