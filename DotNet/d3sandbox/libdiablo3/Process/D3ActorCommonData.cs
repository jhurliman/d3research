using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Process
{
    public class D3ActorCommonData
    {
        /// <summary>Pointer to the ActorCommonData in memory</summary>
        public uint Pointer;
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

        public D3ActorCommonData(MemoryReader memReader, uint ptr, byte[] data)
        {
            this.Pointer = ptr;
            this.ACDID = BitConverter.ToUInt32(data, 0);
            this.ModelName = ProcessUtils.AsciiBytesToString(data, 4, 128);
            this.GBType = BitConverter.ToInt32(data, 176);
            this.GBID = BitConverter.ToUInt32(data, 180);
            this.AttributesID = BitConverter.ToUInt32(data, 288);
            this.AttributesPtr = memReader.IDToPtr(memReader.pAttributes, Offsets.SIZEOF_ATTRIBUTE, AttributesID);
        }
    }
}
