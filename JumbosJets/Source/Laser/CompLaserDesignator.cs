using System.Collections.Generic;
using RimWorld;
using Verse;

namespace JumbosJets
{
    public class CompLaserDesignator : ThingComp
    {
        private const int MaxCharges = 25;
        private int chargesRemaining = MaxCharges;
        private int ticksRemaining;
        private Pawn user;
        private IntVec3 targetCell = IntVec3.Invalid;
        private IntVec3 startCell = IntVec3.Invalid;
        private LaserBeamMote beamMote;
        private Verb aimVerb;
        private Stance_Cooldown laserStance;

        public bool Active => user != null && targetCell.IsValid;
        public bool IsAiming => Active && user.Drafted && laserStance != null &&
            user.stances.curStance == laserStance;
        public IntVec3 TargetCell => targetCell;
        public Pawn User => user;
        public int ChargesRemaining => chargesRemaining;

        public bool CanUse => chargesRemaining > 0;

        public override string CompInspectStringExtra()
        {
            return $"Charges: {chargesRemaining}/{MaxCharges}";
        }

        public void ConsumeCharge()
        {
            if (chargesRemaining > 0)
                chargesRemaining--;
        }

        public void Begin(Pawn pawn, IntVec3 cell, Verb verb)
        {
            if (Active)
                Stop();
            user = pawn;
            targetCell = cell;
            startCell = pawn.Position;
            aimVerb = verb;
            ticksRemaining = (props as CompProperties_LaserDesignator)?.activeDurationTicks ?? 1200;
            LaserDesignatorTracker.Set(this, pawn.Map, cell);
            CreateOrUpdateBeam(pawn, cell);
        }

        public void Stop()
        {
            if (laserStance != null && user?.stances?.curStance == laserStance)
                user.stances.SetStance(new Stance_Mobile());
            laserStance = null;
            aimVerb = null;
            LaserDesignatorTracker.Clear(this);
            if (beamMote != null && !beamMote.Destroyed)
                beamMote.Destroy(DestroyMode.Vanish);
            beamMote = null;
            user = null;
            targetCell = IntVec3.Invalid;
            startCell = IntVec3.Invalid;
            ticksRemaining = 0;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!Active)
                return;

            if (ticksRemaining-- <= 0)
            {
                Stop();
                return;
            }

            Apparel apparel = parent as Apparel;
            bool stillEquipped = apparel != null
                ? user.apparel != null && user.apparel.WornApparel.Contains(apparel)
                : user.equipment != null && user.equipment.Primary == parent;
            if (user.Dead || user.Downed || user.Map == null || user.Position != startCell ||
                !user.Drafted || !stillEquipped ||
                !targetCell.InBounds(user.Map) || !GenSight.LineOfSight(user.Position, targetCell, user.Map))
            {
                Stop();
                return;
            }

            if (laserStance == null)
            {
                laserStance = new Stance_Cooldown(ticksRemaining, new LocalTargetInfo(targetCell), aimVerb);
                user.stances.SetStance(laserStance);
            }
            else if (user.stances.curStance != laserStance)
            {
                Stop();
                return;
            }

            CreateOrUpdateBeam(user, targetCell);

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;
            if (Active)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Stop laser designator",
                    defaultDesc = "Stop pointing the laser designator.",
                    action = Stop
                };
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref chargesRemaining, "laserCharges", MaxCharges);
            Scribe_References.Look(ref user, "laserUser");
            Scribe_Values.Look(ref targetCell, "laserTargetCell", IntVec3.Invalid);
            Scribe_Values.Look(ref startCell, "laserStartCell", IntVec3.Invalid);
            Scribe_Values.Look(ref ticksRemaining, "laserTicksRemaining", 0);
        }

        private void CreateOrUpdateBeam(Pawn pawn, IntVec3 cell)
        {
            if (pawn?.Map == null || !cell.IsValid)
                return;

            if (beamMote == null || beamMote.Destroyed)
            {
                ThingDef beamDef = DefDatabase<ThingDef>.GetNamedSilentFail("JJ_LaserBeam");
                if (beamDef == null)
                    return;
                beamMote = ThingMaker.MakeThing(beamDef) as LaserBeamMote;
                if (beamMote == null)
                    return;
                GenSpawn.Spawn(beamMote, pawn.Position, pawn.Map);
            }

            beamMote.SetEndpoints(this, pawn, cell);
        }
    }
}
