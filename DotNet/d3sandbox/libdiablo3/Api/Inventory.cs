using System;
using System.Collections.Generic;

namespace libdiablo3.Api
{
    public class Inventory
    {
        public readonly Item[,] Slots;
        
        private List<Item> items;

        internal Inventory(int columns, int slotCount)
        {
            int slotsX = columns;
            int slotsY = slotCount / columns;

            Slots = new Item[slotsY, slotsX];
            items = new List<Item>();
        }

        public Item[] GetItems()
        {
            return items.ToArray();
        }

        public int EmptySlotCount()
        {
            int empty = 0;

            for (int y = 0; y < Slots.GetLength(0); y++)
            {
                for (int x = 0; x < Slots.GetLength(1); x++)
                {
                    if (Slots[y, x] == null)
                        ++empty;
                }
            }

            return empty;
        }

        public bool HasEmpty1x2Slot()
        {
            for (int y = 0; y < Slots.GetLength(0) - 1; y++)
            {
                for (int x = 0; x < Slots.GetLength(1); x++)
                {
                    if (Slots[y, x] == null && Slots[y + 1, x] == null)
                        return true;
                }
            }

            return false;
        }

        internal void AddItem(Item item)
        {
            Vector2i dimensions = item.InventorySize;
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                    Slots[item.InventoryY + y, item.InventoryX + x] = item;
            }

            if (!items.Contains(item))
                items.Add(item);
        }

        public override string ToString()
        {
            int filledSlots = Slots.GetLength(0) * Slots.GetLength(1) - EmptySlotCount();
            return String.Format("{0} items in {1} slots", items.Count, filledSlots);
        }
    }
}
