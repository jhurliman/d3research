using System;
using System.Collections.Generic;
using System.Text;

namespace d3sandbox
{
    public class InvalidHostException : Exception
    {
        public InvalidHostException(string message) : base(message) {}
    }

    public static class Utils
    {
        public static uint HashItemName(string input)
        {
            input = input.ToLowerInvariant();

            uint hash = 0;
            for (int i = 0; i < input.Length; i++)
                hash = (hash << 5) + hash + input[i];
            return hash;
        }

        public static string BitsToString(uint bits)
        {
            return Convert.ToString(bits, 2).PadLeft(32, '0');
        }

        public static string AsciiBytesToString(this byte[] buffer, int offset, int maxLength)
        {
            int maxIndex = offset + maxLength;

            for (int i = offset; i < maxIndex; i++)
            {
                /// Skip non-nulls.
                if (buffer[i] != 0) continue;
                /// First null we find, return the string.
                return Encoding.ASCII.GetString(buffer, offset, i - offset);
            }
            /// Terminating null not found. Convert the entire section from offset to maxLength.
            return Encoding.ASCII.GetString(buffer, offset, maxLength);
        }
    }
}
