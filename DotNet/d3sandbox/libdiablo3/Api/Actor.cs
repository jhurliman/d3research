using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class Actor : IEquatable<Actor>
    {
        public readonly ActorName Type;
        public readonly ActorCategory Category;
        public readonly TeamType Team;
        public int InstanceID;
        public int AcdID;
        public AABB BoundingBox;
        public Vector2f Direction;

        internal Actor(int snoID, int category, int team)
        {
            Type = (ActorName)snoID;
            Category = (ActorCategory)category;
            Team = (TeamType)team;
        }

        public Vector3f Position { get { return BoundingBox.Center; } }

        public override int GetHashCode()
        {
            return InstanceID;
        }

        public bool Equals(Actor actor)
        {
            return this.InstanceID == actor.InstanceID;
        }

        public override bool Equals(object obj)
        {
            Actor actor = obj as Actor;
            if (actor != null)
                return Equals(actor);
            return false;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        internal static Actor CreateInstance(Actor template, int instanceID, int acdID, AABB aabb,
            Vector2f direction)
        {
            Actor actor = template.MemberwiseClone() as Actor;
            actor.InstanceID = instanceID;
            actor.AcdID = acdID;
            actor.BoundingBox = aabb;
            actor.Direction = direction;
            return actor;
        }
    }
}
