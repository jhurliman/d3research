using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class Item : Actor
    {
        internal Item(int snoID, int teamID)
            : base(snoID, (int)ActorCategory.Item, teamID)
        {
        }
    }
}
