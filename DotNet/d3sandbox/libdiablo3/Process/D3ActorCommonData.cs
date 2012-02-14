using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Process
{
    public class D3ActorCommonData : IEquatable<D3ActorCommonData>
    {
        /// <summary>Pointer to the ActorCommonData in memory</summary>
        public uint Pointer;
        /// <summary>Unique lookup ID for this ActorCommonData</summary>
        public int AcdID;
        /// <summary>3D model name</summary>
        public string ModelName;
        /// <summary>SNO file associated with this actor</summary>
        public int SnoID;
        /// <summary>Game balance type</summary>
        public int GBType;
        /// <summary>Game balance ID. A hash of the original game balance
        /// string identifier</summary>
        public uint GBID;
        /// <summary>Normalized facing direction</summary>
        public Vector2f Direction;
        /// <summary>AABB.Min</summary>
        public Vector3f Pos1;
        /// <summary>AABB.Max</summary>
        public Vector3f Pos2;
        /// <summary>World that this actor exists in</summary>
        public uint WorldID;
        /// <summary>Scene that this actor exists in</summary>
        public uint SceneID;
        /// <summary>ID of the actor that owns this object</summary>
        public int OwnerID;
        /// <summary>Where this item is located, if it is an item belonging to
        /// a player</summary>
        public int Placement;
        /// <summary>X position of this item if it is in an inventory</summary>
        public int InventoryX;
        /// <summary>Y position of this item if it is in an inventory</summary>
        public int InventoryY;
        /// <summary>Attributes ID, used to find the collection of attributes 
        /// attached to this ACD</summary>
        public uint AttributesID1;
        /// <summary>Unknown, potentially stores additional attributes</summary>
        public uint AttributesID2;
        /// <summary>Collection of attributes attached to this ACD</summary>
        public D3AttributeMap Attributes;

        public D3ActorCommonData(MemoryReader memReader, uint ptr, byte[] data)
        {
            this.Pointer = ptr;
            this.AcdID = BitConverter.ToInt32(data, 0);
            this.ModelName = ProcessUtils.AsciiBytesToString(data, 4, 128);
            this.SnoID = BitConverter.ToInt32(data, 0x90);
            this.GBType = BitConverter.ToInt32(data, 0xB0);
            this.GBID = BitConverter.ToUInt32(data, 0xB4);
            this.Direction = new Vector2f(data, 0xC8);
            this.Pos1 = new Vector3f(data, 0xD0);
            this.Pos2 = new Vector3f(data, 0xE0);
            this.WorldID = BitConverter.ToUInt32(data, 0x108);
            this.SceneID = BitConverter.ToUInt32(data, 0x10C);
            this.OwnerID = BitConverter.ToInt32(data, 0x110);
            this.Placement = BitConverter.ToInt32(data, 0x114);
            this.InventoryX = BitConverter.ToInt32(data, 0x118);
            this.InventoryY = BitConverter.ToInt32(data, 0x11C);
            this.AttributesID1 = BitConverter.ToUInt32(data, 0x120);
            this.AttributesID2 = BitConverter.ToUInt32(data, 0x124);

            uint attributesPtr1 = memReader.IDToPtr(memReader.pAttributes, Offsets.SIZEOF_ATTRIBUTE, AttributesID1);
            //uint attributesPtr2 = memReader.IDToPtr(memReader.pAttributes, Offsets.SIZEOF_ATTRIBUTE, AttributesID2);

            Attributes = memReader.GetAttributes(attributesPtr1);

            // Merge in any additional attributes (haven't seen any yet in practice)
            //D3AttributeMap attributes2 = memReader.GetAttributes(attributesPtr2);
            //foreach (var kvp in attributes2)
            //    Attributes[kvp.Key] = kvp.Value;
        }

        public override string ToString()
        {
            return String.IsNullOrEmpty(ModelName) ? "(Unknown)" : ModelName;
        }

        public bool Equals(D3ActorCommonData acd)
        {
            return this.Pointer == acd.Pointer && this.AcdID == acd.AcdID;
        }

        public override bool Equals(object obj)
        {
            if (obj is D3ActorCommonData)
                return Equals((D3ActorCommonData)obj);
            return false;
        }

        public static bool operator ==(D3ActorCommonData lhs, D3ActorCommonData rhs)
        {
            if (System.Object.ReferenceEquals(lhs, rhs))
                return true;
            if (((object)lhs == null) || ((object)rhs == null))
                return false;
            return lhs.Equals(rhs);
        }

        public static bool operator !=(D3ActorCommonData lhs, D3ActorCommonData rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return (int)AcdID;
        }
    }
}
