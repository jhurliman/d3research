using System;
using System.Collections.Generic;
using System.Text;

namespace d3sandbox
{
    public enum ActorType
    {
        Other,
        Environment,
        ACDBased
    }

    /// <summary>
    /// In-game actor instance
    /// </summary>
    public class RActor
    {
        /// <summary>Pointer to the RActor in memory</summary>
        public uint Pointer;
        /// <summary>Unique lookup ID for this actor</summary>
        public uint ActorID;
        /// <summary>Unique lookup ID for this actor's ActorCommonData</summary>
        public uint AcdID;
        /// <summary>3D model name</summary>
        public string ModelName;
        /// <summary>SNO file associated with this actor</summary>
        public uint SnoID;
        /// <summary>Normalized facing direction</summary>
        public Vector2 Direction;
        /// <summary>MinAABB?</summary>
        public Vector3 Pos1;
        /// <summary>Scale?</summary>
        public float Unk2;
        /// <summary>MaxAABB?</summary>
        public Vector3 Pos2;
        /// <summary>World that this actor exists in</summary>
        public uint WorldID;
        /// <summary>Environment pointer?</summary>
        public uint Unk3;

        public RActor(uint ptr, byte[] data)
        {
            this.Pointer = ptr;
            this.ActorID = BitConverter.ToUInt32(data, 0);
            this.AcdID = BitConverter.ToUInt32(data, 4);
            this.ModelName = Utils.AsciiBytesToString(data, 8, 128);
            this.SnoID = BitConverter.ToUInt32(data, 136);
            this.Direction = new Vector2(data, 152);
            this.Pos1 = new Vector3(data, 160);
            this.Unk2 = BitConverter.ToSingle(data, 172);
            this.Pos2 = new Vector3(data, 176);
            this.WorldID = BitConverter.ToUInt32(data, 216);
            this.Unk3 = BitConverter.ToUInt32(data, 344);
        }

        public ActorType Type
        {
            get
            {
                if (AcdID != MemoryReader.INVALID)
                    return ActorType.ACDBased;
                if (Unk3 == MemoryReader.INVALID)
                    return ActorType.Other;
                return ActorType.Environment;
            }
        }
    }

    /// <summary>
    /// In-game instance of actor common data
    /// </summary>
    public class ActorCommonData
    {
        /// <summary>Unique lookup ID for this ActorCommonData</summary>
        public uint ACDID;
        /// <summary>3D model name</summary>
        public string ModelName;
        /// <summary>Game balance type</summary>
        public int GBType;
        /// <summary>Game balance ID. A hash of the original game balance
        /// string identifier</summary>
        public uint GBID;
        /// <summary>Attribute ID, used to find the collection of attributes 
        /// attached to this ACD</summary>
        public uint AttributeID;

        public ActorCommonData(byte[] data)
        {
            this.ACDID = BitConverter.ToUInt32(data, 0);
            this.ModelName = Utils.AsciiBytesToString(data, 4, 128);
            this.GBType = BitConverter.ToInt32(data, 176);
            this.GBID = BitConverter.ToUInt32(data, 180);
            this.AttributeID = BitConverter.ToUInt32(data, 288);
        }
    }
}
