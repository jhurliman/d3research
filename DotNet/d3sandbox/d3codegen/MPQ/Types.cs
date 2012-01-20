using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using CrystalMpq;
using Gibbed.IO;
using Mooege.Common.MPQ;
using Mooege.Common.Storage;
using Mooege.Net.GS.Message;

namespace Mooege.Core.GS.Common.Types.Math
{
    public class Vector3D : ISerializableData
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3D()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public Vector3D(Vector3D vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        public Vector3D(float x, float y, float z)
        {
            Set(x, y, z);
        }

        /// <summary>
        /// Reads Vector3D from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public Vector3D(MpqFileStream stream)
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
        public float DistanceSquared(ref Vector3D point)
        {
            float x = point.X - X;
            float y = point.Y - Y;
            float z = point.Z - Z;

            return ((x * x) + (y * y)) + (z * z);
        }

        public static bool operator ==(Vector3D a, Vector3D b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            return a.Equals(b);
        }

        public static bool operator !=(Vector3D a, Vector3D b)
        {
            return !(a == b);
        }

        public static bool operator >(Vector3D a, Vector3D b)
        {
            if (object.ReferenceEquals(null, a))
                return !object.ReferenceEquals(null, b);
            return a.X > b.X
                && a.Y > b.Y
                && a.Z > b.Z;
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static bool operator <(Vector3D a, Vector3D b)
        {
            return !(a > b);
        }

        public static bool operator >=(Vector3D a, Vector3D b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            return a.X >= b.X
                && a.Y >= b.Y
                && a.Z >= b.Z;
        }

        public static bool operator <=(Vector3D a, Vector3D b)
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
            var v = o as Vector3D;
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
            return string.Format("x:{0} y:{1} z:{2}", X, Y, Z);
        }
    }

    public class Vector2D
    {
        [PersistentProperty("X")]
        public int X;

        [PersistentProperty("Y")]
        public int Y;

        public Vector2D() { }

        /// <summary>
        /// Reads Vector2D from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public Vector2D(MpqFileStream stream)
        {
            X = stream.ReadValueS32();
            Y = stream.ReadValueS32();
        }

        public Vector2D(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Parses Vector2D from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            X = buffer.ReadInt(32);
            Y = buffer.ReadInt(32);
        }

        /// <summary>
        /// Encodes Vector2D to given GameBitBuffer.
        /// </summary>        
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, X);
            buffer.WriteInt(32, Y);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("Vector2D:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("X: 0x" + X.ToString("X8") + " (" + X + ")");
            b.Append(' ', pad);
            b.AppendLine("Y: 0x" + Y.ToString("X8") + " (" + Y + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1}", X, Y);
        }
    }

    public struct Vector2F : IEquatable<Vector2F>
    {
        public float X;
        public float Y;
        private static Vector2F _zero;
        private static Vector2F _one;
        private static Vector2F _unitX;
        private static Vector2F _unitY;

        public static Vector2F Zero
        {
            get { return _zero; }
        }

        public static Vector2F One
        {
            get { return _one; }
        }

        public static Vector2F UnitX
        {
            get { return _unitX; }
        }

        public static Vector2F UnitY
        {
            get { return _unitY; }
        }

        public Vector2F(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2F(float value)
        {
            this.X = this.Y = value;
        }

        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{{X:{0} Y:{1}}}",
                                 new object[] { this.X.ToString(currentCulture), this.Y.ToString(currentCulture) });
        }

        public bool Equals(Vector2F other)
        {
            return ((this.X == other.X) && (this.Y == other.Y));
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Vector2F)
            {
                flag = this.Equals((Vector2F)obj);
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return (this.X.GetHashCode() + this.Y.GetHashCode());
        }

        public float Length()
        {
            float num = (this.X * this.X) + (this.Y * this.Y);
            return (float)System.Math.Sqrt((double)num);
        }

        public float LengthSquared()
        {
            return ((this.X * this.X) + (this.Y * this.Y));
        }

        public static float Distance(Vector2F value1, Vector2F value2)
        {
            float num2 = value1.X - value2.X;
            float num = value1.Y - value2.Y;
            float num3 = (num2 * num2) + (num * num);
            return (float)System.Math.Sqrt((double)num3);
        }

        public static void Distance(ref Vector2F value1, ref Vector2F value2, out float result)
        {
            float num2 = value1.X - value2.X;
            float num = value1.Y - value2.Y;
            float num3 = (num2 * num2) + (num * num);
            result = (float)System.Math.Sqrt((double)num3);
        }

        public static float DistanceSquared(Vector2F value1, Vector2F value2)
        {
            float num2 = value1.X - value2.X;
            float num = value1.Y - value2.Y;
            return ((num2 * num2) + (num * num));
        }

        public static void DistanceSquared(ref Vector2F value1, ref Vector2F value2, out float result)
        {
            float num2 = value1.X - value2.X;
            float num = value1.Y - value2.Y;
            result = (num2 * num2) + (num * num);
        }

        public static float Dot(Vector2F value1, Vector2F value2)
        {
            return ((value1.X * value2.X) + (value1.Y * value2.Y));
        }

        public static void Dot(ref Vector2F value1, ref Vector2F value2, out float result)
        {
            result = (value1.X * value2.X) + (value1.Y * value2.Y);
        }

        public void Normalize()
        {
            float num2 = (this.X * this.X) + (this.Y * this.Y);
            float num = 1f / ((float)System.Math.Sqrt((double)num2));
            this.X *= num;
            this.Y *= num;
        }

        public static Vector2F Normalize(Vector2F value)
        {
            Vector2F vector;
            float num2 = (value.X * value.X) + (value.Y * value.Y);
            float num = 1f / ((float)System.Math.Sqrt((double)num2));
            vector.X = value.X * num;
            vector.Y = value.Y * num;
            return vector;
        }

        public static void Normalize(ref Vector2F value, out Vector2F result)
        {
            float num2 = (value.X * value.X) + (value.Y * value.Y);
            float num = 1f / ((float)System.Math.Sqrt((double)num2));
            result.X = value.X * num;
            result.Y = value.Y * num;
        }

        /// <summary>
        /// Returns the angle in radians between this vector and another vector
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float Angle(Vector2F other)
        {
            return (float)System.Math.Acos(Dot(this, other) / Length() / other.Length());
        }

        /// <summary>
        /// Returns the rotation of this vector to the x unity vector in radians
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float Rotation()
        {
            return Angle(Vector2F.UnitY) > Angle(-Vector2F.UnitY) ? -Angle(Vector2F.UnitX) : Angle(Vector2F.UnitX);
        }


        public static Vector2F Reflect(Vector2F vector, Vector2F normal)
        {
            Vector2F vector2F;
            float num = (vector.X * normal.X) + (vector.Y * normal.Y);
            vector2F.X = vector.X - ((2f * num) * normal.X);
            vector2F.Y = vector.Y - ((2f * num) * normal.Y);
            return vector2F;
        }

        public static void Reflect(ref Vector2F vector, ref Vector2F normal, out Vector2F result)
        {
            float num = (vector.X * normal.X) + (vector.Y * normal.Y);
            result.X = vector.X - ((2f * num) * normal.X);
            result.Y = vector.Y - ((2f * num) * normal.Y);
        }

        public static Vector2F Min(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = (value1.X < value2.X) ? value1.X : value2.X;
            vector.Y = (value1.Y < value2.Y) ? value1.Y : value2.Y;
            return vector;
        }

        public static void Min(ref Vector2F value1, ref Vector2F value2, out Vector2F result)
        {
            result.X = (value1.X < value2.X) ? value1.X : value2.X;
            result.Y = (value1.Y < value2.Y) ? value1.Y : value2.Y;
        }

        public static Vector2F Max(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = (value1.X > value2.X) ? value1.X : value2.X;
            vector.Y = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            return vector;
        }

        public static void Max(ref Vector2F value1, ref Vector2F value2, out Vector2F result)
        {
            result.X = (value1.X > value2.X) ? value1.X : value2.X;
            result.Y = (value1.Y > value2.Y) ? value1.Y : value2.Y;
        }

        public static Vector2F Clamp(Vector2F value1, Vector2F min, Vector2F max)
        {
            Vector2F vector;
            float x = value1.X;
            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;
            float y = value1.Y;
            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;
            vector.X = x;
            vector.Y = y;
            return vector;
        }

        public static void Clamp(ref Vector2F value1, ref Vector2F min, ref Vector2F max, out Vector2F result)
        {
            float x = value1.X;
            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;
            float y = value1.Y;
            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;
            result.X = x;
            result.Y = y;
        }

        public static Vector2F Lerp(Vector2F value1, Vector2F value2, float amount)
        {
            Vector2F vector;
            vector.X = value1.X + ((value2.X - value1.X) * amount);
            vector.Y = value1.Y + ((value2.Y - value1.Y) * amount);
            return vector;
        }

        public static void Lerp(ref Vector2F value1, ref Vector2F value2, float amount, out Vector2F result)
        {
            result.X = value1.X + ((value2.X - value1.X) * amount);
            result.Y = value1.Y + ((value2.Y - value1.Y) * amount);
        }

        public static Vector2F Negate(Vector2F value)
        {
            Vector2F vector;
            vector.X = -value.X;
            vector.Y = -value.Y;
            return vector;
        }

        public static void Negate(ref Vector2F value, out Vector2F result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        public static Vector2F Add(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = value1.X + value2.X;
            vector.Y = value1.Y + value2.Y;
            return vector;
        }

        public static void Add(ref Vector2F value1, ref Vector2F value2, out Vector2F result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }

        public static Vector2F Subtract(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = value1.X - value2.X;
            vector.Y = value1.Y - value2.Y;
            return vector;
        }

        public static void Subtract(ref Vector2F value1, ref Vector2F value2, out Vector2F result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
        }

        public static Vector2F Multiply(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = value1.X * value2.X;
            vector.Y = value1.Y * value2.Y;
            return vector;
        }

        public static void Multiply(ref Vector2F value1, ref Vector2F value2, out Vector2F result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
        }

        public static Vector2F Multiply(Vector2F value1, float scaleFactor)
        {
            Vector2F vector;
            vector.X = value1.X * scaleFactor;
            vector.Y = value1.Y * scaleFactor;
            return vector;
        }

        public static void Multiply(ref Vector2F value1, float scaleFactor, out Vector2F result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
        }

        public static Vector2F Divide(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = value1.X / value2.X;
            vector.Y = value1.Y / value2.Y;
            return vector;
        }

        public static void Divide(ref Vector2F value1, ref Vector2F value2, out Vector2F result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
        }

        public static Vector2F Divide(Vector2F value1, float divider)
        {
            Vector2F vector;
            float num = 1f / divider;
            vector.X = value1.X * num;
            vector.Y = value1.Y * num;
            return vector;
        }

        public static void Divide(ref Vector2F value1, float divider, out Vector2F result)
        {
            float num = 1f / divider;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
        }

        public static Vector2F operator -(Vector2F value)
        {
            Vector2F vector;
            vector.X = -value.X;
            vector.Y = -value.Y;
            return vector;
        }

        public static bool operator ==(Vector2F value1, Vector2F value2)
        {
            return ((value1.X == value2.X) && (value1.Y == value2.Y));
        }

        public static bool operator !=(Vector2F value1, Vector2F value2)
        {
            if (value1.X == value2.X)
            {
                return !(value1.Y == value2.Y);
            }
            return true;
        }

        public static Vector2F operator +(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = value1.X + value2.X;
            vector.Y = value1.Y + value2.Y;
            return vector;
        }

        public static Vector2F operator -(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = value1.X - value2.X;
            vector.Y = value1.Y - value2.Y;
            return vector;
        }

        public static Vector2F operator *(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = value1.X * value2.X;
            vector.Y = value1.Y * value2.Y;
            return vector;
        }

        public static Vector2F operator *(Vector2F value, float scaleFactor)
        {
            Vector2F vector;
            vector.X = value.X * scaleFactor;
            vector.Y = value.Y * scaleFactor;
            return vector;
        }

        public static Vector2F operator *(float scaleFactor, Vector2F value)
        {
            Vector2F vector;
            vector.X = value.X * scaleFactor;
            vector.Y = value.Y * scaleFactor;
            return vector;
        }

        public static Vector2F operator /(Vector2F value1, Vector2F value2)
        {
            Vector2F vector;
            vector.X = value1.X / value2.X;
            vector.Y = value1.Y / value2.Y;
            return vector;
        }

        public static Vector2F operator /(Vector2F value1, float divider)
        {
            Vector2F vector;
            float num = 1f / divider;
            vector.X = value1.X * num;
            vector.Y = value1.Y * num;
            return vector;
        }

        static Vector2F()
        {
            _zero = new Vector2F();
            _one = new Vector2F(1f, 1f);
            _unitX = new Vector2F(1f, 0f);
            _unitY = new Vector2F(0f, 1f);
        }
    }

    public class Quaternion
    {
        public float W;
        public Vector3D Vector3D;

        public Quaternion() { }

        /// <summary>
        /// Reads Quaternion from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public Quaternion(MpqFileStream stream)
        {
            this.Vector3D = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.W = stream.ReadValueF32();
        }

        /// <summary>
        /// Parses Quaternion from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            W = buffer.ReadFloat32();
            Vector3D = new Vector3D();
            Vector3D.Parse(buffer);
        }

        /// <summary>
        /// Encodes Quaternion to given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(W);
            Vector3D.Encode(buffer);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("Quaternion:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("W: " + W.ToString("G"));
            Vector3D.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

    public class PRTransform
    {
        public Quaternion Quaternion;
        public Vector3D Vector3D;

        public PRTransform() { }

        /// <summary>
        /// Reads PRTransform from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public PRTransform(MpqFileStream stream)
        {
            Quaternion = new Quaternion(stream);
            Vector3D = new Vector3D(stream);
        }

        /// <summary>
        /// Reads PRTransform from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            Quaternion = new Quaternion();
            Quaternion.Parse(buffer);
            Vector3D = new Vector3D();
            Vector3D.Parse(buffer);
        }

        /// <summary>
        /// Encodes PRTransform to given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            Quaternion.Encode(buffer);
            Vector3D.Encode(buffer);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PRTransform:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Quaternion.AsText(b, pad);
            Vector3D.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

    public class AABB
    {
        public Vector3D Min;
        public Vector3D Max;

        public AABB() { }

        /// <summary>
        /// Reads AABB from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public AABB(MpqFileStream stream)
        {
            this.Min = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.Max = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
        }

        /// <summary>
        /// Parses AABB from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            Min = new Vector3D();
            Min.Parse(buffer);
            Max = new Vector3D();
            Max.Parse(buffer);
        }

        /// <summary>
        /// Encodes AABB to given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            Min.Encode(buffer);
            Max.Encode(buffer);
        }

        public bool IsWithin(Vector3D v)
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

    public class RGBAColor
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Alpha;

        public RGBAColor() { }

        /// <summary>
        /// Reads RGBAColor from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public RGBAColor(MpqFileStream stream)
        {
            var buf = new byte[4];
            stream.Read(buf, 0, 4);
            Red = buf[0];
            Green = buf[1];
            Blue = buf[2];
            Alpha = buf[3];
        }

        /// <summary>
        /// Parses RGBAColor from given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to parse from.</param>
        public void Parse(GameBitBuffer buffer)
        {
            Red = (byte)buffer.ReadInt(8);
            Green = (byte)buffer.ReadInt(8);
            Blue = (byte)buffer.ReadInt(8);
            Alpha = (byte)buffer.ReadInt(8);
        }

        /// <summary>
        /// Encodes RGBAColor to given GameBitBuffer.
        /// </summary>
        /// <param name="buffer">The GameBitBuffer to write.</param>
        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(8, Red);
            buffer.WriteInt(8, Green);
            buffer.WriteInt(8, Blue);
            buffer.WriteInt(8, Alpha);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RGBAColor:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Reg: 0x" + Red.ToString("X2"));
            b.Append(' ', pad);
            b.AppendLine("Green: 0x" + Green.ToString("X2"));
            b.Append(' ', pad);
            b.AppendLine("Blue: 0x" + Blue.ToString("X2"));
            b.Append(' ', pad);
            b.AppendLine("Alpha: 0x" + Alpha.ToString("X2"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
