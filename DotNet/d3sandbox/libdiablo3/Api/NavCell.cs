using System;

namespace libdiablo3.Api
{
    [Flags]
    public enum NavCellFlags : int
    {
        AllowWalk = 0x1,
        AllowFlier = 0x2,
        AllowSpider = 0x4,
        LevelAreaBit0 = 0x8,
        LevelAreaBit1 = 0x10,
        NoNavMeshIntersected = 0x20,
        NoSpawn = 0x40,
        Special0 = 0x80,
        Special1 = 0x100,
        SymbolNotFound = 0x200,
        AllowProjectile = 0x400,
        AllowGhost = 0x800,
        RoundedCorner0 = 0x1000,
        RoundedCorner1 = 0x2000,
        RoundedCorner2 = 0x4000,
        RoundedCorner3 = 0x8000
    }

    public struct NavCell
    {
        public AABB BoundingBox;
        public NavCellFlags Flags;

        public NavCell(float minX, float minY, float minZ, float maxX, float maxY, float maxZ, int flags)
        {
            BoundingBox = new AABB(
                new Vector3f(minX, minY, minZ),
                new Vector3f(maxX, maxY, maxZ));
            Flags = (NavCellFlags)flags;
        }
    }
}
