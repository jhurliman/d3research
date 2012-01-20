using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class Gizmo : Actor
    {
        public readonly GizmoGroup GizmoType;

        public Gizmo(int snoID, int gizmoGroup, int teamID)
            : base(snoID, (int)ActorCategory.Gizmo, teamID)
        {
            GizmoType = (GizmoGroup)gizmoGroup;
        }

        internal Gizmo CreateInstance(int instanceID, AABB aabb, Vector2f direction,
            int gizmoGroup)
        {
            Gizmo gizmo = this.MemberwiseClone() as Gizmo;
            gizmo.InstanceID = instanceID;
            gizmo.BoundingBox = aabb;
            gizmo.Direction = direction;
            return gizmo;
        }
    }
}
