using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CrystalMpq;
using Gibbed.IO;

namespace d3sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2I
    {
        public int X;
        public int Y;

        /// <summary>
        /// Reads Vector2D from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public Vector2I(MpqFileStream stream)
        {
            X = stream.ReadValueS32();
            Y = stream.ReadValueS32();
        }

        public Vector2I(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("Vector2I:");
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
            return String.Format("<{0}, {1}>", X, Y);
        }
    }
}
