using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class SkillSlots
    {
        private Skill walk = new Skill(PowerName.Walk, 0f);
        private Skill[] slots = new Skill[9];
        
        public Skill Walk { get { return walk; } }
        public Skill LeftMouse { get { return slots[0]; } }
        public Skill RightMouse { get { return slots[1]; } }
        // TODO: What is slots[2]?
        // TODO: What is slots[3]?
        public Skill Key1 { get { return slots[4]; } }
        public Skill Key2 { get { return slots[5]; } }
        public Skill Key3 { get { return slots[6]; } }
        public Skill Key4 { get { return slots[7]; } }
        public Skill Key5 { get { return slots[8]; } }

        public SkillSlots() { }

        public bool HasSkill(Skill skill)
        {
            if (skill == walk)
                return true;

            for (int i = 0; i < slots.Length; i++)
            {
                if (skill == slots[i])
                    return true;
            }
            return false;
        }

        internal void UpdateSkills(uint[] slots)
        {
            // FIXME: Need to pass in an array of structs that hold powerIDs and cooldown times
            for (int i = 0; i < slots.Length; i++)
                this.slots[i] = new Skill((PowerName)slots[i], 0f);
        }
    }
}
