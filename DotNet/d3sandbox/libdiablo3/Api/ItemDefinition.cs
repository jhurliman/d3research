using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Api
{
    public class ItemDefinition
    {
        public readonly ItemQuality Quality;
        public readonly int ItemLevel;
        public readonly int RequiredLevel;
        public readonly int BaseGoldValue;
        public readonly int MaxSockets;
        public readonly int MaxStackAmount;
        public readonly int ItemTypeHash;

        public ItemDefinition(int quality, int itemLevel, int requiredLevel, int baseGoldValue, int maxSockets, int maxStackAmount, int itemTypeHash)
        {
            Quality = (ItemQuality)quality;
            ItemLevel = itemLevel;
            RequiredLevel = requiredLevel;
            BaseGoldValue = baseGoldValue;
            MaxSockets = maxSockets;
            MaxStackAmount = maxStackAmount;
            ItemTypeHash = itemTypeHash;
        }
    }
}
