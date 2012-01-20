using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    #region Monster Enums

    public enum MonsterCategory : int
    {
        Unknown = -1,
        Big = 3,
        Standard = 4,
        Ranged = 5,
        Swarm = 6,
        Boss = 7
    }

    public enum MonsterRace : int
    {
        Unknown = -1,
        Fallen = 1,
        GoatMen = 2,
        Rogue = 3,
        Skeleton = 4,
        Zombie = 5,
        Spider = 6,
        Triune = 7,
        WoodWraith = 8,
        Human = 9,
        Animal = 10
    }

    public enum MonsterType : int
    {
        Undead = 0,
        Demon = 1,
        Beast = 2,
        Human = 3,
        Breakable = 4,
        Scenery = 5,
        Ally = 6,
        Team = 7,
        Helper = 8
    }

    public enum MonsterPowerType : int
    {
        Mana = 0,
        Arcanum = 1,
        Fury = 2,
        Spirit = 3,
        Power = 4,
        Hatred = 5,
        Discipline = 6
    }

    public enum Resistance : int
    {
        Physical = 0,
        Fire = 1,
        Lightning = 2,
        Cold = 3,
        Poison = 4,
        Arcane = 5,
        Holy = 6
    }

    #endregion Monster Enums

    public class Monster : Actor
    {
        public readonly MonsterCategory MonsterCategory;
        public readonly MonsterRace MonsterRace;
        public readonly MonsterType MonsterType;
        public readonly MonsterPowerType PowerType;
        public readonly Resistance Resists;

        public int Level { get; internal set; }
        public int ExperienceGranted { get; internal set; }
        public int HitpointsCurrent { get; internal set; }
        public int HitpointsMax { get; internal set; }

        public Monster(int snoID, int monsterCategory, int race, int type, int powerType, 
            int resists)
            : base(snoID, (int)ActorCategory.Monster, (int)TeamType.Hostile)
        {
            MonsterCategory = (MonsterCategory)monsterCategory;
            MonsterRace = (MonsterRace)race;
            MonsterType = (MonsterType)type;
            PowerType = (MonsterPowerType)powerType;
            Resists = (Resistance)resists;
        }

        internal Monster CreateInstance(int instanceID, AABB aabb, Vector2f direction, 
            int level, int xpGranted, int hpCur, int hpMax)
        {
            Monster monster = this.MemberwiseClone() as Monster;
            monster.InstanceID = instanceID;
            monster.BoundingBox = aabb;
            monster.Direction = direction;
            monster.Level = level;
            monster.ExperienceGranted = xpGranted;
            monster.HitpointsCurrent = hpCur;
            monster.HitpointsMax = hpMax;
            return monster;
        }
    }
}
