using System.Collections.Generic;
using Verse;

namespace JumbosJets
{
    public static class LaserGuidance
    {
        public const int Radius = 10;
        private const int RadiusSquared = Radius * Radius;

        public static bool GuidesRun(Map map, IList<IntVec3> line)
        {
            if (map == null || line == null || line.Count == 0)
                return false;

            foreach (KeyValuePair<CompLaserDesignator, IntVec3> designation in LaserDesignatorTracker.ForMap(map))
                foreach (IntVec3 cell in line)
                    if (WithinRadius(cell, designation.Value))
                        return true;
            return false;
        }

        public static List<IntVec3> PreviewCells(Map map)
        {
            var cells = new HashSet<IntVec3>();
            if (map == null)
                return new List<IntVec3>();

            foreach (KeyValuePair<CompLaserDesignator, IntVec3> designation in LaserDesignatorTracker.ForMap(map))
            {
                IntVec3 center = designation.Value;
                for (int x = -Radius; x <= Radius; x++)
                    for (int z = -Radius; z <= Radius; z++)
                    {
                        var cell = new IntVec3(center.x + x, 0, center.z + z);
                        if (cell.InBounds(map) && WithinRadius(cell, center))
                            cells.Add(cell);
                    }
            }
            return new List<IntVec3>(cells);
        }

        private static bool WithinRadius(IntVec3 cell, IntVec3 center)
        {
            int dx = cell.x - center.x;
            int dz = cell.z - center.z;
            return dx * dx + dz * dz <= RadiusSquared;
        }
    }
}
