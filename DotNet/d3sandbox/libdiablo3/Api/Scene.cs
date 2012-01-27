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

        public Scene(string name, bool active, uint sceneID, uint worldID, int snoID, AABB aabb)
        {
            this.Name = name;
            this.Active = active;
            this.SceneID = sceneID;
            this.WorldID = worldID;
            this.NavCells = Api.NavCells.SceneNavCells[snoID];
            this.BoundingBox = aabb;
        }
    }
}
