using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Process
{
    public class D3MovementInfo
    {
        public uint VTable;
        public bool IsMoving;
        public int PathComplexity;
        public Vector3f Target;
        public Vector3f Direction;

        public D3MovementInfo(uint ptr, byte[] data)
        {
            VTable = BitConverter.ToUInt32(data, 0);
            IsMoving = BitConverter.ToBoolean(data, 0x34);
            PathComplexity = BitConverter.ToInt32(data, 0x38);
            Target = new Vector3f(data, 0x3C);
            Direction = new Vector3f(data, 0x68);
        }
    }
}
