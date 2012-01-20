using System;
using System.Collections.Generic;
using System.Text;
using CrystalMpq;

namespace d3sandbox
{
    public struct RGBAColor
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Alpha;

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
