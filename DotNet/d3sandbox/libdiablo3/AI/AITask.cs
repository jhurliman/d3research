using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libdiablo3.Api;

namespace libdiablo3.AI
{
    public abstract class AITask
    {
        public AIState State;
        public DateTime Started;
        public DateTime LastStarted;

        public AITask(AIState state)
        {
            State = state;
        }

        public abstract double GetPriority();
        public abstract PowerInfo GetAction();
    }
}
