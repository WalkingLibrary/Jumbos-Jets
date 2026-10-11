using RimWorld;
using UnityEngine;
using Verse;

namespace JumbosJets
{
    public class Verb_LaserDesignator : Verb_Shoot
    {
        protected override bool TryCastShot()
        {
            Pawn pawn = CasterPawn;
            Thing equipment = FindLaserSource(pawn);
            if (pawn == null || equipment == null || !currentTarget.IsValid || !CanHitTarget(currentTarget) ||
                !GenSight.LineOfSight(pawn.Position, currentTarget.Cell, pawn.Map))
                return false;

            CompLaserDesignator comp = equipment.TryGetComp<CompLaserDesignator>();
            if (comp == null)
                return false;

            if (!comp.CanUse)
                return false;

            comp.ConsumeCharge();
            comp.Begin(pawn, currentTarget.Cell, this);
            return true;
        }

        private Thing FindLaserSource(Pawn pawn)
        {
            Thing equipment = EquipmentSource;
            if (equipment == null)
                equipment = pawn?.equipment?.Primary;
            if (equipment != null && equipment.TryGetComp<CompLaserDesignator>() != null)
                return equipment;

            if (pawn?.apparel?.WornApparel == null)
                return null;
            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                if (apparel.TryGetComp<CompLaserDesignator>() != null)
                    return apparel;
            }
            return null;
        }

        public override bool CanHitTarget(LocalTargetInfo target)
        {
            Pawn pawn = CasterPawn;
            return pawn != null && pawn.Map != null && target.IsValid && target.Cell.InBounds(pawn.Map) &&
                pawn.Position.DistanceTo(target.Cell) <= verbProps.range;
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            base.DrawHighlight(target);
        }
    }
}
