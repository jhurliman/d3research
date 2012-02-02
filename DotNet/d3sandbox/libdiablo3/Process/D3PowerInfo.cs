using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libdiablo3.Process
{
    public enum PowerTargetType : int
    {
        ACD = 1,
        Position = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public class D3PowerInfo
    {
        public uint SnoID1;
        public uint SnoID2;
        public PowerTargetType TargetType;
        public uint TargetAcdID;
        public Vector3f TargetPos;
        public uint WorldID;
        public uint Unk3;
        public uint Unk4;

        public D3PowerInfo(uint snoID, Vector3f target, uint worldID)
        {
            SnoID1 = SnoID2 = snoID;
            TargetType = PowerTargetType.Position;
            TargetAcdID = Offsets.INVALID;
            TargetPos = target;
            WorldID = worldID;
            Unk3 = Offsets.INVALID;
            Unk4 = 0;
        }

        public D3PowerInfo(uint snoID, uint targetAcdID)
        {
            SnoID1 = SnoID2 = snoID;
            TargetType = PowerTargetType.ACD;
            TargetAcdID = targetAcdID;
            TargetPos = Vector3f.Zero;
            WorldID = 0;
            Unk3 = Offsets.INVALID;
            Unk4 = 0;
        }
    }
}
