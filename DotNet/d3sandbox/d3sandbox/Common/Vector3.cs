using System;
using System.Collections.Generic;
using System.Text;
using CrystalMpq;
using Gibbed.IO;

namespace d3sandbox
{
    public class Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public Vector3(Vector3 vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        public Vector3(float x, float y, float z)
        {
            Set(x, y, z);
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

        /// <summary>
        /// Parses Vector3D from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            X = buffer.ReadFloat32();
            Y = buffer.ReadFloat32();
            Z = buffer.ReadFloat32();
        }

        /// <summary>
        /// Encodes Vector3D to given GameBitBuffer.
        /// </summary>        
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(X);
            buffer.WriteFloat32(Y);
            buffer.WriteFloat32(Z);
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
            if (object.ReferenceEquals(this, o))
                return true;
            var v = o as Vector3;
            if (v != null)
            {
                return this.X == v.X
                    && this.Y == v.Y
                    && this.Z == v.Z;
            }
            return false;
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
