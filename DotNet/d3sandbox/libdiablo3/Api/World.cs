using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class World
    {
        public Player Me { get; internal set; }
        public Scene[] Scenes { get; internal set; }
        public Hero[] Heros { get; internal set; }
        public Monster[] Monsters { get; internal set; }
        public NPC[] NPCs { get; internal set; }
        public Item[] Items { get; internal set; }
        public Gizmo[] Gizmos { get; internal set; }
        public Actor[] OtherActors { get; internal set; }

        private WorldActorEnumerator enumerator;

        public IEnumerable<Actor> AllActors { get { return enumerator; } }

        public World()
        {
            enumerator = new WorldActorEnumerator(this);
        }

        public Actor GetClosestWaypoint()
        {
            // FIXME:
            return null;
        }

        public Monster GetClosestMonster()
        {
            // FIXME:
            return null;
        }

        #region Enumerator

        public class WorldActorEnumerator : IEnumerable<Actor>
        {
            private World world;

            public WorldActorEnumerator(World world)
            {
                this.world = world;
            }

            public IEnumerator<Actor> GetEnumerator()
            {
                if (world.Me != null)
                    yield return world.Me;

                if (world.Heros == null)
                    yield break;

                foreach (Hero hero in world.Heros)
                    yield return hero;
                foreach (Monster monster in world.Monsters)
                    yield return monster;
                foreach (Item item in world.Items)
                    yield return item;
                foreach (Gizmo gizmo in world.Gizmos)
                    yield return gizmo;
                foreach (Actor actor in world.OtherActors)
                    yield return actor;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion Enumerator
    }
}
