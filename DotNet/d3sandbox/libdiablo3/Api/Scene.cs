using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class Scene
    {
        /// <summary>Name of the scene</summary>
        public readonly string Name;
        /// <summary>Whether this scene is currently active or not</summary>
        public readonly bool Active;
        /// <summary>Scene identifier</summary>
        public readonly uint SceneID;
        /// <summary>Identifier for the world this scene belongs to</summary>
        public readonly uint WorldID;
        /// <summary>Navigation cells for this scene</summary>
        public readonly NavCell[] NavCells;
        /// <summary>Bounding box for this scene</summary>
        public readonly AABB BoundingBox;

        public Scene(string name, bool active, uint sceneID, uint worldID, int snoID, Vector3f position)
        {
            this.Name = name;
            this.Active = active;
            this.SceneID = sceneID;
            this.WorldID = worldID;
            this.NavCells = Api.NavCells.SceneNavCells[snoID];

            this.BoundingBox = new AABB(position, Vector3f.MinValue);
            for (int i = 0; i < this.NavCells.Length; i++)
            {
                AABB aabb = this.NavCells[i].BoundingBox;
                aabb.Max += position;
                if (aabb.Max.X > BoundingBox.Max.X) BoundingBox.Max.X = aabb.Max.X;
                if (aabb.Max.Y > BoundingBox.Max.Y) BoundingBox.Max.Y = aabb.Max.Y;
                if (aabb.Max.Z > BoundingBox.Max.Z) BoundingBox.Max.Z = aabb.Max.Z;
            }
        }
    }
}
