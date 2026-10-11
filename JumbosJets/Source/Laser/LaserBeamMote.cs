using Verse;
using UnityEngine;

namespace JumbosJets
{
    // A spawned mote is drawn during the map's world render pass each frame.
    // MoteDualAttached normally draws an icon, so draw the actual beam here.
    public class LaserBeamMote : MoteDualAttached
    {
        private CompLaserDesignator owner;
        private Pawn source;
        private IntVec3 targetCell = IntVec3.Invalid;

        public void SetEndpoints(CompLaserDesignator designator, Pawn pawn, IntVec3 cell)
        {
            owner = designator;
            source = pawn;
            targetCell = cell;
        }

        protected override void Tick()
        {
            // Lifetime and endpoint updates are controlled by the owning comp.
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (owner == null || !owner.IsAiming || source == null || !source.Spawned || source.Map != Map ||
                !targetCell.IsValid || !targetCell.InBounds(Map))
                return;

            GenDraw.DrawLineBetween(
                source.DrawPos,
                targetCell.ToVector3Shifted(),
                SimpleColor.Green,
                0.24f);
        }
    }
}
