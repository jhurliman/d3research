using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace d3sandbox
{
    /// <summary>Hero classes</summary>
    public enum HeroClass
    {
        Unknown = -1,
        Barbarian = 0,
        DemonHunter,
        Monk,
        WitchDoctor,
        Wizard
    }

    /// <summary>Type of in-game entity</summary>
    public enum EntityType
    {
        Other = -1,
        Monster = 1,
        Gizmo = 2,
        ClientEffect = 3,
        ServerProp = 4,
        Environment = 5,
        Critter = 6,
        Player = 7,
        Item = 8,
        AxeSymbol = 9,
        Projectile = 10,
        CustomBrain = 11
    }

    public enum Team
    {
        None = 0,
        NPC = 1,
        Ally = 2,
        Hostile = 10
    }

    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(byte[] data, int pos)
        {
            X = BitConverter.ToSingle(data, pos);
            Y = BitConverter.ToSingle(data, pos + 4);
        }

        public override string ToString()
        {
            return String.Format("<{0:0.000}, {1:0.000}>", X, Y);
        }
    }

    public class Entity
    {
        public string Name;
        public string ModelName;
        public EntityType Type;
        public Vector3 Position;
        public Vector2 Direction;
        public Dictionary<GameAttribute, GameAttributeValue> Attributes;

        internal RActor _rActor;
        internal ActorCommonData _acd;

        public Entity(RActor actor, ActorCommonData acd)
        {
            _rActor = actor;
            _acd = acd;

            Name = "";
            ModelName = _rActor.ModelName;
            Type = (EntityType)_acd.GBType;
            Position = _rActor.Pos1;
            Direction = _rActor.Direction;
        }

        public override string ToString()
        {
            string str = String.Format("[{0}] {1} ({2}) @ {3} {4} ({5} attributes)", Type, Name, ModelName, Position, Direction,
                Attributes != null ? Attributes.Count : 0);
            if (Attributes.Count != 0)
            {
                foreach (KeyValuePair<GameAttribute, GameAttributeValue> kvp in Attributes)
                    str += Environment.NewLine + ' ' + kvp.Key.Name + '=' + kvp.Key.ValueToString(kvp.Value);
            }
            return str;
        }
    }

    public class Player : Entity
    {
        public HeroClass Class;
        public int CurrentExperience;
        public int NextLevelExperience;

        public Player(RActor actor, ActorCommonData acd)
            : base(actor, acd)
        {
            Class = MemoryReader.GBIDToClass(acd.GBID);
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1} ({2}) @ {3} {4}", Class, Name, ModelName, Position, Direction);
        }
    }
}
