using Verse;

namespace JumbosJets
{
    public class CompProperties_LaserDesignator : CompProperties
    {
        public int activeDurationTicks = 1200;

        public CompProperties_LaserDesignator()
        {
            compClass = typeof(CompLaserDesignator);
        }
    }
}
