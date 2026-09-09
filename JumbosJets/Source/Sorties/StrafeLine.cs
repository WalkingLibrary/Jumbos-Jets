using System;
using System.Collections.Generic;
using Verse;

namespace JumbosJets
{
    public static class StrafeLine
    {
        public static int ImpactLimit(int maxImpacts, int maxRunCells, int selectedCells)
        {
            if (maxImpacts <= 0 || maxRunCells <= 0 || selectedCells <= 0)
                return 0;
            // A short run represents a proportional part of the full burst.
            // Round down so the configured maximum is never exceeded.
            return Math.Min(maxImpacts, Math.Max(1, maxImpacts * selectedCells / maxRunCells));
        }

        // Bresenham's line, truncated from the first selected square. The cap
        // counts grid squares, including the start, equally in every direction.
        public static List<IntVec3> Cells(IntVec3 start, IntVec3 end, int maxCells)
        {
            var cells = new List<IntVec3>();
            if (!start.IsValid || !end.IsValid || maxCells <= 0)
                return cells;
            int x = start.x;
            int z = start.z;
            int dx = Math.Abs(end.x - x);
            int dz = -Math.Abs(end.z - z);
            int stepX = x < end.x ? 1 : -1;
            int stepZ = z < end.z ? 1 : -1;
            int error = dx + dz;
            while (cells.Count < maxCells)
            {
                cells.Add(new IntVec3(x, 0, z));
                if (x == end.x && z == end.z)
                    break;
                int twiceError = 2 * error;
                if (twiceError >= dz)
                {
                    error += dz;
                    x += stepX;
                }
                if (twiceError <= dx)
                {
                    error += dx;
                    z += stepZ;
                }
            }
            return cells;
        }
    }
}
