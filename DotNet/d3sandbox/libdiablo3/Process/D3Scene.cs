using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Process
{
    public class D3Scene
    {
        /// <summary>Pointer to the scene in memory</summary>
        public uint Pointer;
        /// <summary>Unique lookup ID for this scene</summary>
        public uint InstanceID;
        /// <summary>Scene identifier</summary>
        public uint SceneID;
        /// <summary>Identifier for the world this scene belongs to</summary>
        public uint WorldID;
        /// <summary>SNO file associated with this scene</summary>
        public uint SnoID;
        /// <summary>Pointer to the NavMesh for this scene</summary>
        public uint NavMeshPtr;
        /// <summary>Whether this scene is currently active or not</summary>
        public bool Active;
        /// <summary>Name of the scene</summary>
        public string Name;

        public D3Scene(MemoryReader memReader, uint ptr, byte[] data)
        {
            this.Pointer = ptr;
            this.InstanceID = BitConverter.ToUInt32(data, 0);
            this.SceneID = BitConverter.ToUInt32(data, 4);
            this.WorldID = BitConverter.ToUInt32(data, 8);
            this.SnoID = BitConverter.ToUInt32(data, 220);
            this.NavMeshPtr = BitConverter.ToUInt32(data, 380);
            this.Active = (memReader.D3.ReadUInt(ptr + 396) & 1) != 0;
            this.Name = memReader.D3.ReadASCIIString(BitConverter.ToUInt32(data, 604) + 8, 128);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1}) ID: {2:X} Instance: {3:X} World: {4:X}",
                Name, Active ? "Active" : "Inactive", SceneID, InstanceID, WorldID);
        }
    }
}
