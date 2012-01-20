using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CrystalMpq;
using Gibbed.IO;

namespace d3sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(Vector3 vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3(byte[] data, int pos)
        {
            this.X = BitConverter.ToSingle(data, pos + 0);
            this.Y = BitConverter.ToSingle(data, pos + 4);
            this.Z = BitConverter.ToSingle(data, pos + 8);
        }

        /// <summary>
        /// Reads Vector3D from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public Vector3(MpqFileStream stream)
            : this(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32())
        {
        }

        public void Read(MpqFileStream stream)
        {
            X = stream.ReadValueF32();
            Y = stream.ReadValueF32();
            Z = stream.ReadValueF32();
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("Vector3D:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("X: " + X.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Y: " + Y.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Z: " + Z.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

        public void Set(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Calculates the distance squared from this vector to another.
        /// </summary>
        /// <param name="point">the second <see cref="Vector3" /></param>
        /// <returns>the distance squared between the vectors</returns>
        public float DistanceSquared(ref Vector3 point)
        {
            float x = point.X - X;
            float y = point.Y - Y;
            float z = point.Z - Z;

            return ((x * x) + (y * y)) + (z * z);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            return a.Equals(b);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        public static bool operator >(Vector3 a, Vector3 b)
        {
            if (object.ReferenceEquals(null, a))
                return !object.ReferenceEquals(null, b);
            return a.X > b.X
                && a.Y > b.Y
                && a.Z > b.Z;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.X / b, a.Y / b, a.Z / b);
        }

        public static bool operator <(Vector3 a, Vector3 b)
        {
            return !(a > b);
        }

        public static bool operator >=(Vector3 a, Vector3 b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            return a.X >= b.X
                && a.Y >= b.Y
                && a.Z >= b.Z;
        }

        public static bool operator <=(Vector3 a, Vector3 b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            return a.X <= b.X
                && a.Y <= b.Y
                && a.Z <= b.Z;
        }

        public override bool Equals(object o)
        {
            if (!(o is Vector3))
                return false;

            var v = (Vector3)o;
            return this.X == v.X
                && this.Y == v.Y
                && this.Z == v.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("<{0}, {1}, {2}>", X, Y, Z);
        }
    }
}
