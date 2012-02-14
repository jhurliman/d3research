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

        public static string AsciiBytesToString(byte[] buffer, int offset, int maxLength)
        {
            int length = maxLength;
            for (int i = offset; i < offset + maxLength; i++)
            {
                if (buffer[i] == 0)
                {
                    length = i - offset;
                    break;
                }
            }

            unsafe
            {
                fixed (byte* pAscii = buffer)
                    return new String((sbyte*)pAscii, offset, length);
            }
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
