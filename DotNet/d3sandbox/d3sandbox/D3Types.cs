using System;
using System.Collections.Generic;
using System.Text;
using Magic;

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
        /// <summary>Attributes ID, used to find the collection of attributes 
        /// attached to this ACD</summary>
        public uint AttributesID;
        /// <summary>Attributes pointer, pointing to the collection of 
        /// attributes attached to this ACD</summary>
        public uint AttributesPtr;

        public ActorCommonData(byte[] data)
        {
            this.ACDID = BitConverter.ToUInt32(data, 0);
            this.ModelName = Utils.AsciiBytesToString(data, 4, 128);
            this.GBType = BitConverter.ToInt32(data, 176);
            this.GBID = BitConverter.ToUInt32(data, 180);
            this.AttributesID = BitConverter.ToUInt32(data, 288);
        }
    }

    public class Scene
    {
        /// <summary>Pointer to the scene in memory</summary>
        public uint Pointer;
        /// <summary>Unique lookup ID for this scene</summary>
        public uint InstanceID;
        /// <summary>Scene identifier</summary>
        public uint SceneID;
        /// <summary>Identifier for the world this scene belongs to</summary>
        public uint WorldID;
        /// <summary>Whether this scene is currently active or not</summary>
        public bool Active;
        /// <summary>Pointer to the NavMesh for this scene</summary>
        public uint NavMeshPtr;
        /// <summary>Name of the scene</summary>
        public string Name;

        //FIXME: Need SnoID. Will be '1904' for scene trOut_Tristram_E09_S14 (0x04000041)

        public Scene(BlackMagic d3, uint ptr, byte[] data)
        {
            this.Pointer = ptr;
            this.InstanceID = BitConverter.ToUInt32(data, 0);
            this.SceneID = BitConverter.ToUInt32(data, 4);
            this.WorldID = BitConverter.ToUInt32(data, 8);
            this.NavMeshPtr = BitConverter.ToUInt32(data, 380);
            this.Active = (d3.ReadUInt(ptr + 396) & 1) != 0;
            this.Name = d3.ReadASCIIString(BitConverter.ToUInt32(data, 604) + 8, 128);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1}) ID: {2:X} Instance: {3:X} World: {4:X}",
                Name, Active ? "Active" : "Inactive", SceneID, InstanceID, WorldID);
        }
    }
}
