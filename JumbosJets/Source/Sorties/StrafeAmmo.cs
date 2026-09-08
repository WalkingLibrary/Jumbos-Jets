using System;
using System.Linq;
using RimWorld;
using Verse;

namespace JumbosJets
{
    public static class StrafeAmmo
    {
        public static int Count(ThingOwner<Thing> inventory)
        {
            return inventory == null ? 0 : inventory
                .Where(thing => thing.def == ThingDefOf.Steel).Sum(thing => thing.stackCount);
        }

        public static bool TryConsume(ThingOwner<Thing> inventory, int amount)
        {
            if (amount <= 0 || Count(inventory) < amount)
                return false;
            int remaining = amount;
            foreach (Thing stack in inventory.Where(thing => thing.def == ThingDefOf.Steel).ToList())
            {
                int count = Math.Min(remaining, stack.stackCount);
                inventory.Take(stack, count).Destroy();
                remaining -= count;
                if (remaining == 0)
                    return true;
            }
            return false;
        }
    }
}
