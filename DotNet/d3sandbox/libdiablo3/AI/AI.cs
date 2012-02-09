using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libdiablo3.Api;

namespace libdiablo3.AI
{
    public class AI
    {
        #region Internal Classes

        private class Interaction : IEquatable<Interaction>
        {
            public PowerName Power;
            public Actor Target;
            public InteractSuccessCallback Success;
            public AIEventCallback Callback;
            public DateTime Started;

            public Interaction(PowerName power, Actor target, InteractSuccessCallback success,
                AIEventCallback callback)
            {
                Power = power;
                Target = target;
                Success = success;
                Callback = callback;
                Started = DateTime.UtcNow;
            }

            public override int GetHashCode()
            {
                return (int)Power ^ Target.InstanceID;
            }

            public bool Equals(Interaction interaction)
            {
                return this.Power == interaction.Power &&
                       this.Target.InstanceID == interaction.Target.InstanceID;
            }

            public override bool Equals(object obj)
            {
                if (obj is Interaction)
                    return Equals((Interaction)obj);
                return false;
            }

            public static bool operator ==(Interaction lhs, Interaction rhs)
            {
                if (System.Object.ReferenceEquals(lhs, rhs))
                    return true;
                if (((object)lhs == null) || ((object)rhs == null))
                    return false;
                return lhs.Equals(rhs);
            }

            public static bool operator !=(Interaction lhs, Interaction rhs)
            {
                return !(lhs == rhs);
            }
        }

        #endregion Internal Classes

        public delegate void AIEventCallback(string err);
        public delegate bool InteractSuccessCallback();

        private Diablo3Api api;
        private Interaction curInteraction;
        private HashSet<Interaction> blacklistedInteractions = new HashSet<Interaction>();

        public AI(Diablo3Api api)
        {
            this.api = api;
        }

        public void BeginMoveTo(Vector3f target, AIEventCallback callback)
        {
        }

        public void BeginMoveTo(Actor target, AIEventCallback callback)
        {
        }

        public void CancelMove()
        {
        }

        public void BeginInteract(World world, PowerName power, Actor target,
            InteractSuccessCallback success, AIEventCallback callback)
        {
            Interaction interaction = new Interaction(power, target, success, callback);

            // If this interaction is blacklisted or in progress, return immediately
            if (blacklistedInteractions.Contains(interaction))
            {
                callback("blacklist");
                return;
            }
            if (curInteraction != null)
            {
                // TODO: Should we allow actions to queue up instead?
                callback("busy");
                return;
            }

            // Register this interaction as in-progress and run it
            curInteraction = interaction;
            world.Me.UsePower(power, target);

            // FIXME: We need an OnWorldUpdate event that will the in-progress interaction
            // and determine if it has completed, timed out, or is still in progress
        }

        public void CancelInteract()
        {
            curInteraction = null;
        }

        /// <summary>
        /// Uses Bresenham's line algorithm to determine if there is a direct
        /// walkable line between the start and end points
        /// </summary>
        /// <param name="world">World containing the start and end points</param>
        /// <param name="start">Starting point</param>
        /// <param name="end">Ending point</param>
        /// <returns>True if there are no walking obstructions between the
        /// start and end points, otherwise false</returns>
        public static bool IsDirectPath(World world, Vector3f start, Vector3f end)
        {
            return IsDirectPath(world, new Vector2f(start.X, start.Y), new Vector2f(end.X, end.Y));
        }

        /// <summary>
        /// Uses Bresenham's line algorithm to determine if there is a direct
        /// walkable line between the start and end points
        /// </summary>
        /// <param name="world">World containing the start and end points</param>
        /// <param name="start">Starting point</param>
        /// <param name="end">Ending point</param>
        /// <returns>True if there are no walking obstructions between the
        /// start and end points, otherwise false</returns>
        public static bool IsDirectPath(World world, Vector2f start, Vector2f end)
        {
            byte[,] map = world.WalkableGrid;
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            Vector2i p0 = new Vector2i((int)(start.X * 0.25f), (int)(start.Y * 0.25f));
            Vector2i p1 = new Vector2i((int)(end.X * 0.25f), (int)(end.Y * 0.25f));
            bool steep = Math.Abs(p1.Y - p0.Y) > Math.Abs(p1.X - p0.X);

            if (steep)
            {
                p0 = new Vector2i(p0.Y, p0.X);
                p1 = new Vector2i(p1.Y, p1.X);
            }

            int deltaX = Math.Abs(p1.X - p0.X);
            int deltaY = Math.Abs(p1.Y - p0.Y);
            int error = 0;
            int deltaError = deltaY;
            int yStep = 0;
            int xStep = 0;
            int y = p0.Y;
            int x = p0.X;

            yStep = (p0.Y < p1.Y) ? 1 : -1;
            xStep = (p0.X < p1.X) ? 1 : -1;

            int tmpX = 0;
            int tmpY = 0;

            while (x != p1.X)
            {
                x += xStep;
                error += deltaError;

                // If the error exceeds the X delta, move one along on the Y axis
                if (2 * error > deltaX)
                {
                    y += yStep;
                    error -= deltaX;
                }

                // Flip the coords back if they're steep
                tmpX = (steep) ? y : x;
                tmpY = (steep) ? x : y;

                if (tmpX >= 0 & tmpX < width & tmpY >= 0 & tmpY < height)
                {
                    // Bail if the cell is unwalkable
                    if (map[tmpX, tmpY] == 0)
                        return false;
                }
                else
                {   // Out of bounds
                    return false;
                }
            }

            return true;
        }
    }
}
