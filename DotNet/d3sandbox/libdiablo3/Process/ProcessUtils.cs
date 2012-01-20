using System;
using System.Collections.Generic;
using System.Text;
using libdiablo3.Api;

namespace libdiablo3.Process
{
    public static class ProcessUtils
    {
        private static Dictionary<uint, HeroType> gbidHeroTypes;

        static ProcessUtils()
        {
            // Initialize the GBID->HeroClass lookup table
            gbidHeroTypes = new Dictionary<uint, HeroType>();
            gbidHeroTypes.Add(HashLowerCase("Barbarian"), HeroType.Barbarian);
            gbidHeroTypes.Add(HashLowerCase("DemonHunter"), HeroType.DemonHunter);
            gbidHeroTypes.Add(HashLowerCase("Monk"), HeroType.Monk);
            gbidHeroTypes.Add(HashLowerCase("WitchDoctor"), HeroType.WitchDoctor);
            gbidHeroTypes.Add(HashLowerCase("Wizard"), HeroType.Wizard);
        }

        public static uint HashLowerCase(string input)
        {
            input = input.ToLowerInvariant();

            uint hash = 0;
            for (int i = 0; i < input.Length; i++)
                hash = (hash << 5) + hash + input[i];
            return hash;
        }

        public static uint HashNormal(string input)
        {
            uint hash = 0;
            for (int i = 0; i < input.Length; ++i)
                hash = (hash << 5) + hash + input[i];
            return hash;
        }

        public static string BytesToHexString(byte[] data)
        {
            StringBuilder output = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
                output.Append(data[i].ToString("X2"));
            return output.ToString();
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

        /// <summary>
        /// Performs a reverse lookup from a hashed game balance ID to the 
        /// original string, as an enum of hero classes
        /// </summary>
        /// <param name="gbid">Hero game balance ID</param>
        public static HeroType GBIDToClass(uint gbid)
        {
            HeroType heroType;
            if (gbidHeroTypes.TryGetValue(gbid, out heroType))
                return heroType;
            return HeroType.Unknown;
        }
    }
}
