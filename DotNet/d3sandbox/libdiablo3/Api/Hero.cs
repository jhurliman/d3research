using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    #region Enums

    public enum HeroType : int
    {
        Unknown = 0,
        Barbarian,
        DemonHunter,
        Monk,
        WitchDoctor,
        Wizard
    }

    public enum ResourceType : int
    {
        None = -1,
        Mana,
        Arcanum,
        Fury,
        Spirit,
        Power,
        Hatred,
        Discipline
    }

    public enum AttributeType : int
    {
        None = -1,
        Strength,
        Dexterity,
        Intelligence,
    }

    #endregion Enums

    public class Hero : Actor
    {
        public readonly HeroType HeroType;

        internal Hero(int snoID)
            : base(snoID, (int)ActorCategory.Player, (int)TeamType.Team)
        {
            string name = ((ActorName)snoID).ToString();

            switch (name.Split('_')[0].ToLower())
            {
                case "barbarian":
                    HeroType = HeroType.Barbarian; break;
                case "demonhunter":
                    HeroType = HeroType.DemonHunter; break;
                case "monk":
                    HeroType = HeroType.Monk; break;
                case "witchdoctor":
                    HeroType = HeroType.WitchDoctor; break;
                case "wizard":
                    HeroType = HeroType.Wizard; break;
            }
        }

        internal static Hero CreateInstance(Hero template, int instanceID, int acdID, AABB aabb,
            Vector2f direction, uint worldID, uint sceneID)
        {
            Hero hero = template.MemberwiseClone() as Hero;
            hero.InstanceID = instanceID;
            hero.AcdID = acdID;
            hero.BoundingBox = aabb;
            hero.Direction = direction;
            hero.WorldID = worldID;
            hero.SceneID = sceneID;
            return hero;
        }
    }
}
