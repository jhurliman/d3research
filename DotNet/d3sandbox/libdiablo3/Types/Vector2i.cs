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
    public struct Vector2i : IEquatable<Vector2i>
    {
        #region Private Fields

        private static Vector2i zeroVector = new Vector2i(0, 0);
        private static Vector2i unitVector = new Vector2i(1, 1);
        private static Vector2i unitXVector = new Vector2i(1, 0);
        private static Vector2i unitYVector = new Vector2i(0, 1);

        #endregion Private Fields


        #region Public Fields

        public int X;
        public int Y;

        #endregion Public Fields


        #region Properties

        public static Vector2i Zero
        {
            get { return zeroVector; }
        }

        public static Vector2i One
        {
            get { return unitVector; }
        }

        public static Vector2i UnitX
        {
            get { return unitXVector; }
        }

        public static Vector2i UnitY
        {
            get { return unitYVector; }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Constructor for standard 2D integer vector.
        /// </summary>
        /// <param name="x">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="y">
        /// A <see cref="System.Int32"/>
        /// </param>
        public Vector2i(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Constructor for "square" vector.
        /// </summary>
        /// <param name="value">
        /// A <see cref="System.Int32"/>
        /// </param>
        public Vector2i(int value)
        {
            this.X = value;
            this.Y = value;
        }

        public Vector2i(byte[] data, int pos)
        {
            this.X = BitConverter.ToInt32(data, pos + 0);
            this.Y = BitConverter.ToInt32(data, pos + 4);
        }

        #endregion Constructors


        #region Public Methods

        public override bool Equals(object obj)
        {
            return (obj is Vector2i) ? this == ((Vector2i)obj) : false;
        }

        public bool Equals(Vector2i other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
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

        public static bool operator ==(Vector2i value1, Vector2i value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }

        public static bool operator !=(Vector2i value1, Vector2i value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }

        #endregion Operators
    }
}