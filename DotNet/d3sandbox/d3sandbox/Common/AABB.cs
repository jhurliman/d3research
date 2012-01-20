using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalMpq;
using Gibbed.IO;

namespace d3sandbox
{
    public class AABB
    {
        public Vector3 Min;
        public Vector3 Max;

        public AABB() { }

        /// <summary>
        /// Reads AABB from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public AABB(MpqFileStream stream)
        {
            this.Min = new Vector3(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.Max = new Vector3(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
        }

        public bool IsWithin(Vector3 v)
        {
            if (v >= this.Min &&
                v <= this.Max)
            {
                return true;
            }
            return false;
        }

        public bool Intersects(AABB other)
        {
            if (// Max < o.Min
                this.Max.X < other.Min.X ||
                this.Max.Y < other.Min.Y ||
                this.Max.Z < other.Min.Z ||
                // Min > o.Max
                this.Min.X > other.Max.X ||
                this.Min.Y > other.Max.Y ||
                this.Min.Z > other.Max.Z)
            {
                return false;
            }
            return true; // Intersects if above fails
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AABB:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Min.AsText(b, pad);
            Max.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

        public override string ToString()
        {
            return string.Format("AABB: min:{0} max:{1}", this.Min, this.Max);
        }
    }
}
