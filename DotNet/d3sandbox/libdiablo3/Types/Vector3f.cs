#region License
/*
MIT License
Copyright Â© 2006 The Mono.Xna Team

All rights reserved.

Authors:
 * Alan McGovern

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;

namespace libdiablo3
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3f : IEquatable<Vector3f>
    {
        #region Private Fields

        private static Vector3f zero = new Vector3f(0f, 0f, 0f);
        private static Vector3f one = new Vector3f(1f, 1f, 1f);
        private static Vector3f unitX = new Vector3f(1f, 0f, 0f);
        private static Vector3f unitY = new Vector3f(0f, 1f, 0f);
        private static Vector3f unitZ = new Vector3f(0f, 0f, 1f);
        private static Vector3f up = new Vector3f(0f, 1f, 0f);
        private static Vector3f down = new Vector3f(0f, -1f, 0f);
        private static Vector3f right = new Vector3f(1f, 0f, 0f);
        private static Vector3f left = new Vector3f(-1f, 0f, 0f);
        private static Vector3f forward = new Vector3f(0f, 0f, -1f);
        private static Vector3f backward = new Vector3f(0f, 0f, 1f);
        private static Vector3f minValue = new Vector3f(Single.MinValue, Single.MinValue, Single.MinValue);
        private static Vector3f maxValue = new Vector3f(Single.MaxValue, Single.MaxValue, Single.MaxValue);

        #endregion Private Fields


        #region Public Fields

        public float X;
        public float Y;
        public float Z;

        #endregion Public Fields


        #region Properties

        public static Vector3f Zero
        {
            get { return zero; }
        }

        public static Vector3f One
        {
            get { return one; }
        }

        public static Vector3f UnitX
        {
            get { return unitX; }
        }

        public static Vector3f UnitY
        {
            get { return unitY; }
        }

        public static Vector3f UnitZ
        {
            get { return unitZ; }
        }

        public static Vector3f Up
        {
            get { return up; }
        }

        public static Vector3f Down
        {
            get { return down; }
        }

        public static Vector3f Right
        {
            get { return right; }
        }

        public static Vector3f Left
        {
            get { return left; }
        }

        public static Vector3f Forward
        {
            get { return forward; }
        }

        public static Vector3f Backward
        {
            get { return backward; }
        }

        public static Vector3f MinValue
        {
            get { return minValue; }
        }

        public static Vector3f MaxValue
        {
            get { return maxValue; }
        }

        #endregion Properties


        #region Constructors

        public Vector3f(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public Vector3f(float value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
        }


        public Vector3f(Vector2f value, float z)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }

        public Vector3f(byte[] data, int pos)
        {
            this.X = BitConverter.ToSingle(data, pos + 0);
            this.Y = BitConverter.ToSingle(data, pos + 4);
            this.Z = BitConverter.ToSingle(data, pos + 8);
        }


        #endregion Constructors


        #region Public Methods

        public static Vector3f Add(Vector3f value1, Vector3f value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        public static void Add(ref Vector3f value1, ref Vector3f value2, out Vector3f result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }

        public static Vector3f Barycentric(Vector3f value1, Vector3f value2, Vector3f value3, float amount1, float amount2)
        {
            return new Vector3f(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
                MathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2));
        }

        public static void Barycentric(ref Vector3f value1, ref Vector3f value2, ref Vector3f value3, float amount1, float amount2, out Vector3f result)
        {
            result = new Vector3f(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
                MathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2));
        }

        public static Vector3f CatmullRom(Vector3f value1, Vector3f value2, Vector3f value3, Vector3f value4, float amount)
        {
            return new Vector3f(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
                MathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount));
        }

        public static void CatmullRom(ref Vector3f value1, ref Vector3f value2, ref Vector3f value3, ref Vector3f value4, float amount, out Vector3f result)
        {
            result = new Vector3f(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
                MathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount));
        }

        public static Vector3f Clamp(Vector3f value1, Vector3f min, Vector3f max)
        {
            return new Vector3f(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y),
                MathHelper.Clamp(value1.Z, min.Z, max.Z));
        }

        public static void Clamp(ref Vector3f value1, ref Vector3f min, ref Vector3f max, out Vector3f result)
        {
            result = new Vector3f(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y),
                MathHelper.Clamp(value1.Z, min.Z, max.Z));
        }

        public static Vector3f Cross(Vector3f vector1, Vector3f vector2)
        {
            Vector3f result;
            result.X = vector1.Y * vector2.Z - vector2.Y * vector1.Z;
            result.Y = vector2.X * vector1.Z - vector1.X * vector2.Z;
            result.Z = vector1.X * vector2.Y - vector2.X * vector1.Y;
            return result;
        }

        public static void Cross(ref Vector3f vector1, ref Vector3f vector2, out Vector3f result)
        {
            result.X = vector1.Y * vector2.Z - vector2.Y * vector1.Z;
            result.Y = vector2.X * vector1.Z - vector1.X * vector2.Z;
            result.Z = vector1.X * vector2.Y - vector2.X * vector1.Y;
        }

        public static float Distance(Vector3f value1, Vector3f value2)
        {
            return (float)Math.Sqrt((value1.X - value2.X) * (value1.X - value2.X) +
                     (value1.Y - value2.Y) * (value1.Y - value2.Y) +
                     (value1.Z - value2.Z) * (value1.Z - value2.Z));
        }

        public static void Distance(ref Vector3f value1, ref Vector3f value2, out float result)
        {
            result = (float)Math.Sqrt((value1.X - value2.X) * (value1.X - value2.X) +
                     (value1.Y - value2.Y) * (value1.Y - value2.Y) +
                     (value1.Z - value2.Z) * (value1.Z - value2.Z));
        }

        public static float DistanceSquared(Vector3f value1, Vector3f value2)
        {
            return (value1.X - value2.X) * (value1.X - value2.X) +
                     (value1.Y - value2.Y) * (value1.Y - value2.Y) +
                     (value1.Z - value2.Z) * (value1.Z - value2.Z); ;
        }

        public static void DistanceSquared(ref Vector3f value1, ref Vector3f value2, out float result)
        {
            result = (value1.X - value2.X) * (value1.X - value2.X) +
                     (value1.Y - value2.Y) * (value1.Y - value2.Y) +
                     (value1.Z - value2.Z) * (value1.Z - value2.Z);
        }

        public static Vector3f Divide(Vector3f value1, Vector3f value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        public static Vector3f Divide(Vector3f value1, float value2)
        {
            float factor = 1.0f / value2;
            value1.X *= factor;
            value1.Y *= factor;
            value1.Z *= factor;
            return value1;
        }

        public static void Divide(ref Vector3f value1, float divisor, out Vector3f result)
        {
            float factor = 1.0f / divisor;
            result.X = value1.X * factor;
            result.Y = value1.Y * factor;
            result.Z = value1.Z * factor;
        }

        public static void Divide(ref Vector3f value1, ref Vector3f value2, out Vector3f result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
        }

        public static float Dot(Vector3f vector1, Vector3f vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        public static void Dot(ref Vector3f vector1, ref Vector3f vector2, out float result)
        {
            result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        public override bool Equals(object obj)
        {
            return (obj is Vector3f) ? this == (Vector3f)obj : false;
        }

        public bool Equals(Vector3f other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return (int)(this.X + this.Y + this.Z);
        }

        public static Vector3f Hermite(Vector3f value1, Vector3f tangent1, Vector3f value2, Vector3f tangent2, float amount)
        {
            value1.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            value1.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
            value1.Z = MathHelper.Hermite(value1.Z, tangent1.Z, value2.Z, tangent2.Z, amount);
            return value1;
        }

        public static void Hermite(ref Vector3f value1, ref Vector3f tangent1, ref Vector3f value2, ref Vector3f tangent2, float amount, out Vector3f result)
        {
            result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
            result.Z = MathHelper.Hermite(value1.Z, tangent1.Z, value2.Z, tangent2.Z, amount);
        }

        public float Length()
        {
            return (float)Math.Sqrt((double)(X * X + Y * Y + Z * Z));
        }

        public float LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public static Vector3f Lerp(Vector3f value1, Vector3f value2, float amount)
        {
            return new Vector3f(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount),
                MathHelper.Lerp(value1.Z, value2.Z, amount));
        }

        public static void Lerp(ref Vector3f value1, ref Vector3f value2, float amount, out Vector3f result)
        {
            result = new Vector3f(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount),
                MathHelper.Lerp(value1.Z, value2.Z, amount));
        }

        public static Vector3f Max(Vector3f value1, Vector3f value2)
        {
            return new Vector3f(
                MathHelper.Max(value1.X, value2.X),
                MathHelper.Max(value1.Y, value2.Y),
                MathHelper.Max(value1.Z, value2.Z));
        }

        public static void Max(ref Vector3f value1, ref Vector3f value2, out Vector3f result)
        {
            result = new Vector3f(
                MathHelper.Max(value1.X, value2.X),
                MathHelper.Max(value1.Y, value2.Y),
                MathHelper.Max(value1.Z, value2.Z));
        }

        public static Vector3f Min(Vector3f value1, Vector3f value2)
        {
            return new Vector3f(
                MathHelper.Min(value1.X, value2.X),
                MathHelper.Min(value1.Y, value2.Y),
                MathHelper.Min(value1.Z, value2.Z));
        }

        public static void Min(ref Vector3f value1, ref Vector3f value2, out Vector3f result)
        {
            result = new Vector3f(
                MathHelper.Min(value1.X, value2.X),
                MathHelper.Min(value1.Y, value2.Y),
                MathHelper.Min(value1.Z, value2.Z));
        }

        public static Vector3f Multiply(Vector3f value1, Vector3f value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        public static Vector3f Multiply(Vector3f value1, float scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            value1.Z *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref Vector3f value1, float scaleFactor, out Vector3f result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        public static void Multiply(ref Vector3f value1, ref Vector3f value2, out Vector3f result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
        }

        public static Vector3f Negate(Vector3f value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            value.Z = -value.Z;
            return value;
        }

        public static void Negate(ref Vector3f value, out Vector3f result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }

        public void Normalize()
        {
            float factor = 1f / (float)Math.Sqrt((double)(X * X + Y * Y + Z * Z));
            X *= factor;
            Y *= factor;
            Z *= factor;
        }

        public static Vector3f Normalize(Vector3f value)
        {
            float factor = 1f / (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y + value.Z * value.Z));
            value.X *= factor;
            value.Y *= factor;
            value.Z *= factor;
            return value;
        }

        public static void Normalize(ref Vector3f value, out Vector3f result)
        {
            float factor = 1f / (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y + value.Z * value.Z));
            result.X = value.X * factor;
            result.Y = value.Y * factor;
            result.Z = value.Z * factor;
        }

        public static Vector3f Reflect(Vector3f vector, Vector3f normal)
        {
            float dotTimesTwo = 2f * Dot(vector, normal);
            vector.X = vector.X - dotTimesTwo * normal.X;
            vector.Y = vector.Y - dotTimesTwo * normal.Y;
            vector.Z = vector.Z - dotTimesTwo * normal.Z;
            return vector;
        }

        public static void Reflect(ref Vector3f vector, ref Vector3f normal, out Vector3f result)
        {
            float dotTimesTwo = 2f * Dot(vector, normal);
            result.X = vector.X - dotTimesTwo * normal.X;
            result.Y = vector.Y - dotTimesTwo * normal.Y;
            result.Z = vector.Z - dotTimesTwo * normal.Z;
        }

        public static Vector3f SmoothStep(Vector3f value1, Vector3f value2, float amount)
        {
            return new Vector3f(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount),
                MathHelper.SmoothStep(value1.Z, value2.Z, amount));
        }

        public static void SmoothStep(ref Vector3f value1, ref Vector3f value2, float amount, out Vector3f result)
        {
            result = new Vector3f(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount),
                MathHelper.SmoothStep(value1.Z, value2.Z, amount));
        }

        public static Vector3f Subtract(Vector3f value1, Vector3f value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        public static void Subtract(ref Vector3f value1, ref Vector3f value2, out Vector3f result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append("{X:");
            sb.Append(this.X);
            sb.Append(" Y:");
            sb.Append(this.Y);
            sb.Append(" Z:");
            sb.Append(this.Z);
            sb.Append("}");
            return sb.ToString();
        }

        public static Vector3f Transform(Vector3f position, Matrix4f matrix)
        {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        public static void Transform(ref Vector3f position, ref Matrix4f matrix, out Vector3f result)
        {
            result = new Vector3f((position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
                                 (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
                                 (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
        }

        public static Vector3f Transform(Vector3f value, Quaternion rotation)
        {
            throw new NotImplementedException();
        }

        public static void Transform(Vector3f[] sourceArray, ref Matrix4f matrix, Vector3f[] destinationArray)
        {
            throw new NotImplementedException();
        }

        public static void Transform(Vector3f[] sourceArray, ref Quaternion rotation, Vector3f[] destinationArray)
        {
            throw new NotImplementedException();
        }

        public static void Transform(Vector3f[] sourceArray, int sourceIndex, ref Matrix4f matrix, Vector3f[] destinationArray, int destinationIndex, int length)
        {
            throw new NotImplementedException();
        }

        public static void Transform(Vector3f[] sourceArray, int sourceIndex, ref Quaternion rotation, Vector3f[] destinationArray, int destinationIndex, int length)
        {
            throw new NotImplementedException();
        }

        public static void Transform(ref Vector3f value, ref Quaternion rotation, out Vector3f result)
        {
            throw new NotImplementedException();
        }

        public static void TransformNormal(Vector3f[] sourceArray, ref Matrix4f matrix, Vector3f[] destinationArray)
        {
            throw new NotImplementedException();
        }

        public static void TransformNormal(Vector3f[] sourceArray, int sourceIndex, ref Matrix4f matrix, Vector3f[] destinationArray, int destinationIndex, int length)
        {
            throw new NotImplementedException();
        }

        public static Vector3f TransformNormal(Vector3f normal, Matrix4f matrix)
        {
            TransformNormal(ref normal, ref matrix, out normal);
            return normal;
        }

        public static void TransformNormal(ref Vector3f normal, ref Matrix4f matrix, out Vector3f result)
        {
            result = new Vector3f((normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
                                 (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
                                 (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33));
        }

        #endregion Public methods


        #region Operators

        public static bool operator ==(Vector3f value1, Vector3f value2)
        {
            return value1.X == value2.X
                && value1.Y == value2.Y
                && value1.Z == value2.Z;
        }

        public static bool operator !=(Vector3f value1, Vector3f value2)
        {
            return value1.X != value2.X
                || value1.Y != value2.Y
                || value1.Z != value2.Z;
        }

        public static Vector3f operator +(Vector3f value1, Vector3f value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        public static Vector3f operator -(Vector3f value)
        {
            value = new Vector3f(-value.X, -value.Y, -value.Z);
            return value;
        }

        public static Vector3f operator -(Vector3f value1, Vector3f value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        public static Vector3f operator *(Vector3f value1, Vector3f value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        public static Vector3f operator *(Vector3f value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        public static Vector3f operator *(float scaleFactor, Vector3f value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        public static Vector3f operator /(Vector3f value1, Vector3f value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        public static Vector3f operator /(Vector3f value, float divider)
        {
            float factor = 1 / divider;
            value.X *= factor;
            value.Y *= factor;
            value.Z *= factor;
            return value;
        }

        #endregion
    }
}