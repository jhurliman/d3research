using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace libdiablo3.Api
{
    public static class NavCells
    {
        public static readonly Dictionary<int, NavCell[]> SceneNavCells;

        static NavCells()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (BinaryReader reader = new BinaryReader(
                assembly.GetManifestResourceStream("libdiablo3.NavCells.bin")))
            {
                SceneNavCells = new Dictionary<int, NavCell[]>();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int snoID = reader.ReadInt32();
                    int cellCount = reader.ReadInt32();

                    NavCell[] navCells = new NavCell[cellCount];

                    for (int i = 0; i < cellCount; i++)
                    {
                        navCells[i] = new NavCell(
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadInt32());
                    }

                    SceneNavCells.Add(snoID, navCells);
                }
            }
        }
    };
};

