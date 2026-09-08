using System.Collections.Generic;
using Verse;

namespace JumbosJets
{
    public class CompProperties_Sortie : CompProperties
    {
        public int maxRunCells = 15;
        public int maxImpacts = 30;
        public int steelPerCell = 1;
        public SoundDef firingSound;
        public SoundDef afterSound;
        public int ticksBetweenImpacts = 6;
        public int approachTicks = 120;
        public int cooldownTicks = 600;
        public float explosionRadius = 1.9f;
        // Unguided default. A future laser-guidance provider can return
        // laserGuidedScatterRadius from CompSortie.GetScatterRadius.
        public float scatterRadius = 2.5f;
        public float laserGuidedScatterRadius = 1.25f;
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
            if (maxRunCells < 1 || maxImpacts < 1 || steelPerCell < 1)
                yield return "Sorties need positive maxRunCells, maxImpacts and steelPerCell.";
            if (firingSound == null || afterSound == null || firingSound.sustain || afterSound.sustain)
                yield return "Sorties need firingSound and afterSound one-shot sound definitions.";
            if (ticksBetweenImpacts < 1 || approachTicks < 1 || cooldownTicks < 1)
                yield return "Sortie timing must be positive.";
            if (explosionRadius <= 0 || damageAmount < 1 || armorPenetration < 0)
                yield return "Sortie explosion settings are invalid.";
            if (float.IsNaN(scatterRadius) || float.IsInfinity(scatterRadius) || scatterRadius < 0)
                yield return "Sortie scatterRadius must be a finite nonnegative number.";
            if (float.IsNaN(laserGuidedScatterRadius) || float.IsInfinity(laserGuidedScatterRadius) || laserGuidedScatterRadius < 0)
                yield return "Sortie laserGuidedScatterRadius must be a finite nonnegative number.";
            if (lowFuelFraction <= 0 || lowFuelFraction >= 1)
                yield return "Sortie lowFuelFraction must be between zero and one.";
        }
    }
}
