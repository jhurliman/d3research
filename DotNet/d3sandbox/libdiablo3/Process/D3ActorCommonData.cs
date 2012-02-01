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
        public uint AttributesID;
        /// <summary>Attributes pointer, pointing to the collection of 
        /// attributes attached to this ACD</summary>
        public uint AttributesPtr;

        public D3ActorCommonData(MemoryReader memReader, uint ptr, byte[] data)
        {
            this.Pointer = ptr;
            this.AcdID = BitConverter.ToInt32(data, 0);
            this.ModelName = ProcessUtils.AsciiBytesToString(data, 4, 128);
            this.SnoID = BitConverter.ToInt32(data, 144);
            this.GBType = BitConverter.ToInt32(data, 176);
            this.GBID = BitConverter.ToUInt32(data, 180);
            this.OwnerID = BitConverter.ToInt32(data, 272);
            this.Placement = BitConverter.ToInt32(data, 276);
            this.InventoryX = BitConverter.ToInt32(data, 280);
            this.InventoryY = BitConverter.ToInt32(data, 284);
            this.AttributesID = BitConverter.ToUInt32(data, 288);
            this.AttributesPtr = memReader.IDToPtr(memReader.pAttributes, Offsets.SIZEOF_ATTRIBUTE, AttributesID);
        }

        public override string ToString()
        {
            return ModelName ?? "(Unknown)";
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

        public override int GetHashCode()
        {
            return (int)AcdID;
        }
    }
}
