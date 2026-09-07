using System.Collections.Generic;
using Verse;

namespace JumbosJets
{
    public class CompProperties_Sortie : CompProperties
    {
        public int burstCount = 9;
        public int cellSpacing = 2;
        public int ticksBetweenImpacts = 6;
        public int approachTicks = 120;
        public int cooldownTicks = 600;
        public float explosionRadius = 1.9f;
        public int damageAmount = 35;
        public float armorPenetration = 0.5f;
        public float lowFuelFraction = 0.2f;

        public CompProperties_Sortie()
        {
            compClass = typeof(CompSortie);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string error in base.ConfigErrors(parentDef))
                yield return error;
            if (burstCount < 1 || burstCount % 2 == 0 || cellSpacing < 1)
                yield return "Sorties need a positive odd burstCount and positive cellSpacing.";
            if (ticksBetweenImpacts < 1 || approachTicks < 1 || cooldownTicks < 1)
                yield return "Sortie timing must be positive.";
            if (explosionRadius <= 0 || damageAmount < 1 || armorPenetration < 0)
                yield return "Sortie explosion settings are invalid.";
            if (lowFuelFraction <= 0 || lowFuelFraction >= 1)
                yield return "Sortie lowFuelFraction must be between zero and one.";
        }
    }
}
