using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class Gizmo : Actor
    {
        public readonly GizmoGroup GizmoType;

        internal Gizmo(int snoID, int gizmoGroup, int teamID)
            : base(snoID, (int)ActorCategory.Gizmo, teamID)
        {
            GizmoType = (GizmoGroup)gizmoGroup;
        }

        internal static Gizmo CreateInstance(Gizmo template, int instanceID, int acdID, AABB aabb,
            Vector2f direction)
        {
            Gizmo gizmo = template.MemberwiseClone() as Gizmo;
            gizmo.InstanceID = instanceID;
            gizmo.AcdID = acdID;
            gizmo.BoundingBox = aabb;
            gizmo.Direction = direction;
            return gizmo;
        }
    }
}
