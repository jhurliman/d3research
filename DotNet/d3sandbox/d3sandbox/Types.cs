using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace d3sandbox
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(byte[] data, int pos)
        {
            X = BitConverter.ToSingle(data, pos);
            Y = BitConverter.ToSingle(data, pos + 4);
        }

        public override string ToString()
        {
            return String.Format("<{0:0.000}, {1:0.000}>", X, Y);
        }
    }

    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(byte[] data, int pos)
        {
            X = BitConverter.ToSingle(data, pos);
            Y = BitConverter.ToSingle(data, pos + 4);
            Z = BitConverter.ToSingle(data, pos + 8);
        }

        public override string ToString()
        {
            return String.Format("<{0:0.000}, {1:0.000}, {2:0.000}>", X, Y, Z);
        }
    }

    public enum ActorType
    {
        Other,
        Environment,
        ACDBased
    }

    public class RActor
    {
        public uint Guid;
        public int Unk1;
        public string ModelName;
        public Vector3 Dir;
        public Vector3 Pos1;
        public float Unk2;
        public Vector3 Pos2;
        public int Unk3;

        public ActorType Type
        {
            get
            {
                if (Unk1 != -1)
                    return ActorType.ACDBased;
                if (Unk3 == -1)
                    return ActorType.Other;
                return ActorType.Environment;
            }
        }

        public RActor(byte[] data)
        {
            this.Guid = BitConverter.ToUInt32(data, 0);
            this.Unk1 = BitConverter.ToInt32(data, 4);
            this.ModelName = Utils.AsciiBytesToString(data, 8, 140);
            this.Dir = new Vector3(data, 148);
            this.Pos1 = new Vector3(data, 160);
            this.Unk2 = BitConverter.ToSingle(data, 172);
            this.Pos2 = new Vector3(data, 176);
            this.Unk3 = BitConverter.ToInt32(data, 344);
        }

        public override string ToString()
        {
            return String.Format("[{8}] GUID={0:X}, Unk1={1:X}, Model={2}, Dir={3}, Pos1={4}, Unk2={5}, Pos2={6}, Unk3={7}",
                Guid, Unk1, ModelName, Dir, Pos1, Unk2, Pos2, Unk3, Type);
        }
    }
}
