using System;
using System.Collections.Generic;
using System.Text;
using CrystalMpq;
using Gibbed.IO;

namespace d3sandbox
{
    public class HardPointLink
    {
        public string Name { get; private set; }
        public int I0 { get; private set; }

        public HardPointLink(MpqFileStream stream)
        {
            this.Name = stream.ReadString(64, true);
            I0 = stream.ReadValueS32();
        }
    }

    public class TriggerConditions
    {
        public int Percent { get; private set; } //0-255
        public int Int1 { get; private set; }
        public int Int2 { get; private set; }
        public int Int3 { get; private set; }
        public int Int4 { get; private set; }
        public int Int5 { get; private set; }
        public int Int6 { get; private set; }
        public int Int7 { get; private set; }
        public int Int8 { get; private set; }

        public TriggerConditions(MpqFileStream stream)
        {
            Percent = stream.ReadByte();
            stream.Position += 3;
            Int1 = stream.ReadValueS32();
            Int2 = stream.ReadValueS32();
            Int3 = stream.ReadValueS32();
            Int4 = stream.ReadValueS32();
            Int5 = stream.ReadValueS32();
            Int6 = stream.ReadValueS32();
            Int7 = stream.ReadValueS32();
            Int8 = stream.ReadValueS32();
        }
    }

    public class TriggerEvent
    {
        public int I0 { get; private set; }
        public TriggerConditions TriggerConditions { get; private set; }
        public int I1 { get; private set; }
        public SNOGroup SnoGroup { get; private set; }
        public int SnoID { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int RuneType { get; private set; }
        public int UseRuneType { get; private set; }
        public HardPointLink[] HardPointLinks { get; private set; }
        public string LookLink { get; private set; }
        public string ConstraintLink { get; private set; }
        public int I4 { get; private set; }
        public float F0 { get; private set; }
        public int I5 { get; private set; }
        public int I6 { get; private set; }
        public int I7 { get; private set; }
        public int I8 { get; private set; }
        public int I9 { get; private set; }
        public float F1 { get; private set; }
        public float F2 { get; private set; }
        public int I10 { get; private set; }
        public float F3 { get; private set; }
        public int I11 { get; private set; }
        public float Velocity { get; private set; }
        public int I12 { get; private set; }
        public int Ticks1 { get; private set; } // DT_TIME
        public RGBAColor Color1 { get; private set; }
        public int I14 { get; private set; } // DT_TIME
        public RGBAColor Color2 { get; private set; }
        public int I15 { get; private set; } // DT_TIME

        public TriggerEvent(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            TriggerConditions = new TriggerConditions(stream);
            I1 = stream.ReadValueS32();
            SnoGroup = (SNOGroup)stream.ReadValueS32();
            SnoID = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            RuneType = stream.ReadValueS32();
            UseRuneType = stream.ReadValueS32();
            HardPointLinks = new HardPointLink[2];
            HardPointLinks[0] = new HardPointLink(stream);
            HardPointLinks[1] = new HardPointLink(stream);
            this.LookLink = stream.ReadString(64, true);
            this.ConstraintLink = stream.ReadString(64, true);
            I4 = stream.ReadValueS32();
            F0 = stream.ReadValueF32();
            I5 = stream.ReadValueS32();
            I6 = stream.ReadValueS32();
            I7 = stream.ReadValueS32();
            I8 = stream.ReadValueS32();
            I9 = stream.ReadValueS32();
            F1 = stream.ReadValueF32();
            F2 = stream.ReadValueF32();
            I10 = stream.ReadValueS32();
            F3 = stream.ReadValueF32();
            I11 = stream.ReadValueS32();
            Velocity = stream.ReadValueF32();
            I12 = stream.ReadValueS32();
            Ticks1 = stream.ReadValueS32();
            Color1 = new RGBAColor(stream);
            I14 = stream.ReadValueS32();
            Color2 = new RGBAColor(stream);
            I15 = stream.ReadValueS32();
        }
    }

    public class MsgTriggeredEvent : ISerializableData
    {
        public int I0 { get; private set; }
        public TriggerEvent TriggerEvent { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            TriggerEvent = new TriggerEvent(stream);
        }
    }
}
