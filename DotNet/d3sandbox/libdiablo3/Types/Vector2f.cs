#region License
/*
MIT License
Copyright Â© 2006 The Mono.Xna Team

All rights reserved.

Authors
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
    public struct Vector2f : IEquatable<Vector2f>
    {
        #region Private Fields

        private static Vector2f zeroVector = new Vector2f(0f, 0f);
        private static Vector2f unitVector = new Vector2f(1f, 1f);
        private static Vector2f unitXVector = new Vector2f(1f, 0f);
        private static Vector2f unitYVector = new Vector2f(0f, 1f);

        #endregion Private Fields


        #region Public Fields

        public float X;
        public float Y;

        #endregion Public Fields


        #region Properties

        public static Vector2f Zero
        {
            get { return zeroVector; }
        }

        public static Vector2f One
        {
            get { return unitVector; }
        }

        public static Vector2f UnitX
        {
            get { return unitXVector; }
        }

        public static Vector2f UnitY
        {
            get { return unitYVector; }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Constructor foe standard 2D vector.
        /// </summary>
        /// <param name="x">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="y">
        /// A <see cref="System.Single"/>
        /// </param>
        public Vector2f(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Constructor for "square" vector.
        /// </summary>
        /// <param name="value">
        /// A <see cref="System.Single"/>
        /// </param>
        public Vector2f(float value)
        {
            this.X = value;
            this.Y = value;
        }

        public Vector2f(byte[] data, int pos)
        {
            this.X = BitConverter.ToSingle(data, pos + 0);
            this.Y = BitConverter.ToSingle(data, pos + 4);
        }

        #endregion Constructors


        #region Public Methods

        public static void Reflect(ref Vector2f vector, ref Vector2f normal, out Vector2f result)
        {
            float dot = Dot(vector, normal);
            result.X = vector.X - ((2f * dot) * normal.X);
            result.Y = vector.Y - ((2f * dot) * normal.Y);
        }

        public static Vector2f Reflect(Vector2f vector, Vector2f normal)
        {
            Vector2f result;
            Reflect(ref vector, ref normal, out result);
            return result;
        }

        public static Vector2f Add(Vector2f value1, Vector2f value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static void Add(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }

        public static Vector2f Barycentric(Vector2f value1, Vector2f value2, Vector2f value3, float amount1, float amount2)
        {
            return new Vector2f(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        public static void Barycentric(ref Vector2f value1, ref Vector2f value2, ref Vector2f value3, float amount1, float amount2, out Vector2f result)
        {
            result = new Vector2f(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        public static Vector2f CatmullRom(Vector2f value1, Vector2f value2, Vector2f value3, Vector2f value4, float amount)
        {
            return new Vector2f(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        public static void CatmullRom(ref Vector2f value1, ref Vector2f value2, ref Vector2f value3, ref Vector2f value4, float amount, out Vector2f result)
        {
            result = new Vector2f(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        public static Vector2f Clamp(Vector2f value1, Vector2f min, Vector2f max)
        {
            return new Vector2f(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y));
        }

        public static void Clamp(ref Vector2f value1, ref Vector2f min, ref Vector2f max, out Vector2f result)
        {
            result = new Vector2f(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y));
        }

        /// <summary>
        /// Returns float precison distanve between two vectors
        /// </summary>
        /// <param name="value1">
        /// A <see cref="Vector2f"/>
        /// </param>
        /// <param name="value2">
        /// A <see cref="Vector2f"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Single"/>
        /// </returns>
        public static float Distance(Vector2f value1, Vector2f value2)
        {
            return (float)Math.Sqrt((value1.X - value2.X) * (value1.X - value2.X) + (value1.Y - value2.Y) * (value1.Y - value2.Y));
        }


        public static void Distance(ref Vector2f value1, ref Vector2f value2, out float result)
        {
            result = (float)Math.Sqrt((value1.X - value2.X) * (value1.X - value2.X) + (value1.Y - value2.Y) * (value1.Y - value2.Y));
        }

        public static float DistanceSquared(Vector2f value1, Vector2f value2)
        {
            return (value1.X - value2.X) * (value1.X - value2.X) + (value1.Y - value2.Y) * (value1.Y - value2.Y);
        }

        public static void DistanceSquared(ref Vector2f value1, ref Vector2f value2, out float result)
        {
            result = (value1.X - value2.X) * (value1.X - value2.X) + (value1.Y - value2.Y) * (value1.Y - value2.Y);
        }

        /// <summary>
        /// Devide first vector with the secund vector
        /// </summary>
        /// <param name="value1">
        /// A <see cref="Vector2f"/>
        /// </param>
        /// <param name="value2">
        /// A <see cref="Vector2f"/>
        /// </param>
        /// <returns>
        /// A <see cref="Vector2f"/>
        /// </returns>
        public static Vector2f Divide(Vector2f value1, Vector2f value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        public static void Divide(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
        }

        public static Vector2f Divide(Vector2f value1, float divider)
        {
            float factor = 1.0f / divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        public static void Divide(ref Vector2f value1, float divider, out Vector2f result)
        {
            float factor = 1.0f / divider;
            result.X = value1.X * factor;
            result.Y = value1.Y * factor;
        }

        public static float Dot(Vector2f value1, Vector2f value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }

        public static void Dot(ref Vector2f value1, ref Vector2f value2, out float result)
        {
            result = value1.X * value2.X + value1.Y * value2.Y;
        }

        public override bool Equals(object obj)
        {
            return (obj is Vector2f) ? this == ((Vector2f)obj) : false;
        }

        public bool Equals(Vector2f other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return (int)(this.X + this.Y);
        }

        public static Vector2f Hermite(Vector2f value1, Vector2f tangent1, Vector2f value2, Vector2f tangent2, float amount)
        {
            value1.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            value1.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
            return value1;
        }

        public static void Hermite(ref Vector2f value1, ref Vector2f tangent1, ref Vector2f value2, ref Vector2f tangent2, float amount, out Vector2f result)
        {
            result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
        }

        public float Length()
        {
            return (float)Math.Sqrt((double)(X * X + Y * Y));
        }

        public float LengthSquared()
        {
            return X * X + Y * Y;
        }

        public static Vector2f Lerp(Vector2f value1, Vector2f value2, float amount)
        {
            return new Vector2f(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount));
        }

        public static void Lerp(ref Vector2f value1, ref Vector2f value2, float amount, out Vector2f result)
        {
            result = new Vector2f(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount));
        }

        public static Vector2f Max(Vector2f value1, Vector2f value2)
        {
            return new Vector2f(
                MathHelper.Max(value1.X, value2.X),
                MathHelper.Max(value1.Y, value2.Y));
        }

        public static void Max(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
        {
            result = new Vector2f(
                MathHelper.Max(value1.X, value2.X),
                MathHelper.Max(value1.Y, value2.Y));
        }

        public static Vector2f Min(Vector2f value1, Vector2f value2)
        {
            return new Vector2f(
                MathHelper.Min(value1.X, value2.X),
                MathHelper.Min(value1.Y, value2.Y));
        }

        public static void Min(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
        {
            result = new Vector2f(
                MathHelper.Min(value1.X, value2.X),
                MathHelper.Min(value1.Y, value2.Y));
        }

        public static Vector2f Multiply(Vector2f value1, Vector2f value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static Vector2f Multiply(Vector2f value1, float scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref Vector2f value1, float scaleFactor, out Vector2f result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
        }

        public static void Multiply(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
        }

        public static Vector2f Negate(Vector2f value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }

        public static void Negate(ref Vector2f value, out Vector2f result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        public void Normalize()
        {
            float factor = 1f / (float)Math.Sqrt((double)(X * X + Y * Y));
            X *= factor;
            Y *= factor;
        }

        public static Vector2f Normalize(Vector2f value)
        {
            float factor = 1f / (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y));
            value.X *= factor;
            value.Y *= factor;
            return value;
        }

        public static void Normalize(ref Vector2f value, out Vector2f result)
        {
            float factor = 1f / (float)Math.Sqrt((double)(value.X * value.X + value.Y * value.Y));
            result.X = value.X * factor;
            result.Y = value.Y * factor;
        }

        public static Vector2f SmoothStep(Vector2f value1, Vector2f value2, float amount)
        {
            return new Vector2f(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount));
        }

        public static void SmoothStep(ref Vector2f value1, ref Vector2f value2, float amount, out Vector2f result)
        {
            result = new Vector2f(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount));
        }

        public static Vector2f Subtract(Vector2f value1, Vector2f value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static void Subtract(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
        }

        public static Vector2f Transform(Vector2f position, Matrix4f matrix)
        {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        public static void Transform(ref Vector2f position, ref Matrix4f matrix, out Vector2f result)
        {
            result = new Vector2f((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41,
                                 (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
        }

        public static Vector2f Transform(Vector2f value, Quaternion rotation)
        {
            throw new NotImplementedException();
        }

        public static void Transform(ref Vector2f value, ref Quaternion rotation, out Vector2f result)
        {
            throw new NotImplementedException();
        }

        public static void Transform(Vector2f[] sourceArray, ref Matrix4f matrix, Vector2f[] destinationArray)
        {
            throw new NotImplementedException();
        }

        public static void Transform(Vector2f[] sourceArray, ref Quaternion rotation, Vector2f[] destinationArray)
        {
            throw new NotImplementedException();
        }

        public static void Transform(Vector2f[] sourceArray, int sourceIndex, ref Matrix4f matrix, Vector2f[] destinationArray, int destinationIndex, int length)
        {
            throw new NotImplementedException();
        }

        public static void Transform(Vector2f[] sourceArray, int sourceIndex, ref Quaternion rotation, Vector2f[] destinationArray, int destinationIndex, int length)
        {
            throw new NotImplementedException();
        }

        public static Vector2f TransformNormal(Vector2f normal, Matrix4f matrix)
        {
            Vector2f.TransformNormal(ref normal, ref matrix, out normal);
            return normal;
        }

        public static void TransformNormal(ref Vector2f normal, ref Matrix4f matrix, out Vector2f result)
        {
            result = new Vector2f((normal.X * matrix.M11) + (normal.Y * matrix.M21),
                                 (normal.X * matrix.M12) + (normal.Y * matrix.M22));
        }

        public static void TransformNormal(Vector2f[] sourceArray, ref Matrix4f matrix, Vector2f[] destinationArray)
        {
            throw new NotImplementedException();
        }

        public static void TransformNormal(Vector2f[] sourceArray, int sourceIndex, ref Matrix4f matrix, Vector2f[] destinationArray, int destinationIndex, int length)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(24);
            sb.Append("{X:");
            sb.Append(this.X);
            sb.Append(" Y:");
            sb.Append(this.Y);
            sb.Append("}");
            return sb.ToString();
        }

        #endregion Public Methods


        #region Operators

        public static Vector2f operator -(Vector2f value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }


        public static bool operator ==(Vector2f value1, Vector2f value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }


        public static bool operator !=(Vector2f value1, Vector2f value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }


        public static Vector2f operator +(Vector2f value1, Vector2f value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }


        public static Vector2f operator -(Vector2f value1, Vector2f value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }


        public static Vector2f operator *(Vector2f value1, Vector2f value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }


        public static Vector2f operator *(Vector2f value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }


        public static Vector2f operator *(float scaleFactor, Vector2f value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }


        public static Vector2f operator /(Vector2f value1, Vector2f value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }


        public static Vector2f operator /(Vector2f value1, float divider)
        {
            float factor = 1 / divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        #endregion Operators
    }
}