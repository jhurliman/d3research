using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class NPC : Actor
    {
        public bool IsOperatable { get; internal set; }
        public int Level { get; internal set; }
        public float HitpointsCurrent { get; internal set; }
        public float HitpointsMax { get; internal set; }

        internal NPC(int snoID, int category, int teamID)
            : base(snoID, category, teamID)
        {
        }

        internal static NPC CreateInstance(NPC template, int instanceID, AABB aabb,
            Vector2f direction, bool isOperatable, int level, float hpCur, float hpMax)
        {
            NPC npc = template.MemberwiseClone() as NPC;
            npc.InstanceID = instanceID;
            npc.BoundingBox = aabb;
            npc.Direction = direction;
            npc.IsOperatable = isOperatable;
            npc.Level = level;
            npc.HitpointsCurrent = hpCur;
            npc.HitpointsMax = hpMax;
            return npc;
        }
    }
}
