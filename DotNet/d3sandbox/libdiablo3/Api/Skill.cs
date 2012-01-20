using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class Skill : IEquatable<Skill>
    {
        public readonly PowerName Power;

        public Skill(PowerName power)
        {
            Power = power;
        }

        public override int GetHashCode()
        {
            return (int)Power;
        }

        public bool Equals(Skill skill)
        {
            return this.Power == skill.Power;
        }

        public override bool Equals(object obj)
        {
            Skill skill = obj as Skill;
            if (skill != null)
                return Equals(skill);
            return false;
        }
    }
}
