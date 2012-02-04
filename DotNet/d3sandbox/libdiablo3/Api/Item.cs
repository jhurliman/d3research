using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libdiablo3.Process;

namespace libdiablo3.Api
{
    #region Enums

    public enum ItemPlacement : int
    {
        None = -1,
        PlayerBackpack = 0,
        PlayerHead = 1,
        PlayerTorso = 2,
        PlayerRightHand = 3,
        PlayerLeftHand = 4,
        PlayerHands = 5,
        PlayerWaist = 6,
        PlayerFeet = 7,
        PlayerShoulders = 8,
        PlayerLegs = 9,
        PlayerBracers = 10,
        PlayerLeftFinger = 11,
        PlayerRightFinger = 12,
        PlayerNeck = 13,
        Unknown1 = 14,
        Unknown2 = 15,
        PlayerStash = 16,
        PlayerGold = 17,
        Unknown3 = 18,
        Merchant = 19,
        Unknown4 = 20,
        Unknown5 = 21,
        PetRightHand = 22,
        PetLeftHand = 23,
        PetSpecial = 24,
        PetNeck = 25,
        PetRightFinger = 26,
        PetLeftFinger = 27,
    }

    [Flags]
    public enum ItemFlags
    {
        NotEquipable1 = 0x1,
        AtLeastMagical = 0x2,
        Gem = 0x8,
        NotEquipable2 = 0x40,
        Socketable = 0x80,
        Unknown = 0x1000,
        Barbarian = 0x100,
        Wizard = 0x200,
        WitchDoctor = 0x400,
        DemonHunter = 0x800,
        Monk = 0x2000,
    }

    public enum ItemQuality
    {
        Invalid = -1,
        Inferior,
        Normal,
        Superior,
        Magic1,
        Magic2,
        Magic3,
        Rare4,
        Rare5,
        Rare6,
        Legendary,
        Artifact,
    }

    #endregion Enums

    #region Helper Classes

    public class ItemType
    {
        public readonly string Name;
        public readonly int Hash;
        public readonly int ParentType;
        public readonly ItemFlags Flags;

        public ItemType BaseType
        {
            get
            {
                var curType = this;
                while (curType.ParentType != -1)
                    curType = ItemTypes.Types[curType.ParentType];
                return curType;
            }
        }

        public ItemType(string name, int hash, int parentType, int flags)
        {
            Name = name;
            Hash = hash;
            ParentType = parentType;
            Flags = (ItemFlags)flags;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    #endregion Helper Classes

    public class Item : Actor
    {
        public ItemType ItemType { get; internal set; }
        public ItemPlacement Placement { get; internal set; }
        public int InventoryX { get; internal set; }
        public int InventoryY { get; internal set; }

        public Vector2i InventorySize
        {
            get
            {
                if (Placement == ItemPlacement.None)
                    return Vector2i.Zero;
                if (Placement == ItemPlacement.Merchant)
                    return Vector2i.One;
                if (Is1x2)
                    return new Vector2i(1, 2);
                return Vector2i.One;
            }
        }

        public bool IsWeapon { get { return IsSubType("Weapon"); } }
        public bool IsArmor { get { return IsSubType("Armor"); } }
        public bool IsOffhand { get { return IsSubType("Offhand"); } }
        public bool Is1x2 { get { return IsWeapon || IsOffhand ||
            (IsArmor && !IsSubType("Belt") && !IsSubType("Ring")); } }

        internal Item(int snoID, int teamID)
            : base(snoID, (int)ActorCategory.Item, teamID)
        {
        }

        internal static Item CreateInstance(Item template, int instanceID, int acdID, AABB aabb,
            Vector2f direction, ItemType type, int placement, int inventoryX, int inventoryY)
        {
            Item item = template.MemberwiseClone() as Item;
            item.ItemType = type;
            item.InstanceID = instanceID;
            item.AcdID = acdID;
            item.BoundingBox = aabb;
            item.Direction = direction;
            item.Placement = (ItemPlacement)placement;
            item.InventoryX = inventoryX;
            item.InventoryY = inventoryY;
            return item;
        }

        private bool IsSubType(string rootTypeName)
        {
            return IsSubType((int)ProcessUtils.HashLowerCase(rootTypeName));
        }

        private bool IsSubType(int rootTypeHash)
        {
            if (ItemType.Hash == rootTypeHash)
                return true;

            var curType = ItemType;
            while (curType.ParentType != -1)
            {
                curType = ItemTypes.Types[curType.ParentType];
                if (curType.Hash == rootTypeHash)
                    return true;
            }

            return false;
        }
    }
}
