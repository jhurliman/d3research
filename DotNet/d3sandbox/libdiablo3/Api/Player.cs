using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class Player : Hero
    {
        public readonly SkillSlots SkillSlots;

        public Player(int snoID)
            : base(snoID)
        {
            SkillSlots = new SkillSlots();
        }
    }
}
