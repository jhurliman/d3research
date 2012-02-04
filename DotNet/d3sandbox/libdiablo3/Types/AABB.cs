using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3
{
    public struct AABB
    {
        public Vector3f Min;
        public Vector3f Max;

        public static readonly AABB Zero = new AABB();

        public AABB(Vector3f min, Vector3f max)
        {
            Min = min;
            Max = max;
        }

        public float Width { get { return Math.Abs(Max.X - Min.X); } }
        public float Height { get { return Math.Abs(Max.Y - Min.Y); } }
        public Vector3f Center { get { return (Min + Max) * 0.5f; } }

        public bool IsWithin(Vector3f v)
        {
            if (// Max < v
                this.Max.X < v.X ||
                this.Max.Y < v.Y ||
                this.Max.Z < v.Z ||
                // Min > v
                this.Min.X > v.X ||
                this.Min.Y > v.Y ||
                this.Min.Z > v.Z)
            {
                return false;
            }
            return true; // Intersects if above fails
        }

        public bool Intersects(AABB other)
        {
            if (// Max < other.Min
                this.Max.X < other.Min.X ||
                this.Max.Y < other.Min.Y ||
                this.Max.Z < other.Min.Z ||
                // Min > other.Max
                this.Min.X > other.Max.X ||
                this.Min.Y > other.Max.Y ||
                this.Min.Z > other.Max.Z)
            {
                return false;
            }
            return true; // Intersects if above fails
        }

        public override string ToString()
        {
            return string.Format("AABB: min:{0} max:{1}", this.Min, this.Max);
        }
    }
}
