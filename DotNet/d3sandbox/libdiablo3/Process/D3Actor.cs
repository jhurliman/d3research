using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magic;

namespace libdiablo3.Process
{
    public class D3Actor
    {
        /// <summary>Pointer to the actor in memory</summary>
        public uint Pointer;
        /// <summary>Unique lookup ID for this actor</summary>
        public int ActorID;
        /// <summary>Unique lookup ID for this actor's ActorCommonData</summary>
        public uint AcdID;
        /// <summary>3D model name</summary>
        public string ModelName;
        /// <summary>SNO file associated with this actor</summary>
        public int SnoID;
        /// <summary>Normalized facing direction</summary>
        public Vector2f Direction;
        /// <summary>MinAABB?</summary>
        public Vector3f Pos1;
        /// <summary>Scale?</summary>
        public float Unk2;
        /// <summary>MaxAABB?</summary>
        public Vector3f Pos2;
        /// <summary>World that this actor exists in</summary>
        public uint WorldID;
        /// <summary>Environment pointer?</summary>
        public uint Unk3;

        public D3ActorCommonData Acd;
        public D3MovementInfo MovementInfo;

        public D3Actor(MemoryReader memReader, uint ptr, byte[] data)
        {
            this.Pointer = ptr;
            this.ActorID = BitConverter.ToInt32(data, 0);
            this.AcdID = BitConverter.ToUInt32(data, 4);
            this.ModelName = ProcessUtils.AsciiBytesToString(data, 8, 128);
            this.SnoID = BitConverter.ToInt32(data, 136);
            this.Direction = new Vector2f(data, 152);
            this.Pos1 = new Vector3f(data, 160);
            this.Unk2 = BitConverter.ToSingle(data, 172);
            this.Pos2 = new Vector3f(data, 176);
            this.WorldID = BitConverter.ToUInt32(data, 216);
            this.Unk3 = BitConverter.ToUInt32(data, 344);

            // ActorCommonData
            if (this.AcdID != Offsets.INVALID)
            {
                uint acdPtr = memReader.IDToPtr(memReader.pACDs, Offsets.SIZEOF_ACD, this.AcdID);
                if (acdPtr != 0 && acdPtr != Offsets.INVALID)
                    this.Acd = new D3ActorCommonData(memReader, acdPtr, memReader.D3.ReadBytes(acdPtr, Offsets.SIZEOF_ACD));
            }

            // MovementInfo
            uint movementPtr = BitConverter.ToUInt32(data, 896);
            if (movementPtr != 0 && movementPtr != Offsets.INVALID)
                this.MovementInfo = new D3MovementInfo(movementPtr, memReader.D3.ReadBytes(movementPtr, Offsets.SIZEOF_MOVEMENTINFO));
        }

        public override string ToString()
        {
            return ModelName;
        }
    }
}
