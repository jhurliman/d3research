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

        internal static Item CreateInstance(Item template, int instanceID, AABB aabb,
            Vector2f direction)
        {
            Item item = template.MemberwiseClone() as Item;
            item.InstanceID = instanceID;
            item.BoundingBox = aabb;
            item.Direction = direction;
            return item;
        }
    }
}
