﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public enum HeroType
    {
        Unknown = 0,
        Barbarian,
        DemonHunter,
        Monk,
        WitchDoctor,
        Wizard
    }

    public class Hero : Actor
    {
        public readonly HeroType HeroType;

        public Hero(int snoID)
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
    }
}
