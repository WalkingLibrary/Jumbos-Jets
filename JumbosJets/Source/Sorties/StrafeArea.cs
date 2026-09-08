using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JumbosJets
{
    public static class StrafeArea
    {
        private static IEnumerable<IntVec3> Disk(IntVec3 center, float radius)
        {
            int extent = (int)Math.Ceiling(radius);
            double squared = radius * radius;
            for (int x = -extent; x <= extent; x++)
                for (int z = -extent; z <= extent; z++)
                    if (x * x + z * z <= squared)
                        yield return new IntVec3(center.x + x, 0, center.z + z);
        }

        private static long Progress(IntVec3 cell, IList<IntVec3> line)
        {
            IntVec3 start = line[0];
            IntVec3 end = line[line.Count - 1];
            return (long)(cell.x - start.x) * (end.x - start.x) +
                (long)(cell.z - start.z) * (end.z - start.z);
        }

        private static IEnumerable<IntVec3> Candidates(IList<IntVec3> line, int index,
            float scatter, Func<IntVec3, bool> canImpact)
        {
            long length = Progress(line[line.Count - 1], line);
            foreach (IntVec3 cell in Disk(line[index], scatter))
            {
                long progress = Progress(cell, line);
                // Scatter widens the run; it cannot move its centers past the
                // selected start/end caps. Single-square runs scatter locally.
                if ((length == 0 || (progress >= 0 && progress <= length)) && canImpact(cell))
                    yield return cell;
            }
        }

        public static List<IntVec3> Footprint(IList<IntVec3> line, IList<float> scatter,
            float explosionRadius, Func<IntVec3, bool> canImpact, Func<IntVec3, bool> inBounds)
        {
            var area = new HashSet<IntVec3>();
            for (int i = 0; i < line.Count; i++)
                foreach (IntVec3 center in Candidates(line, i, scatter[i], canImpact))
                    foreach (IntVec3 cell in Disk(center, explosionRadius))
                        if (inBounds(cell))
                            area.Add(cell);
            return area.ToList();
        }

        public static List<IntVec3> PlanImpacts(IList<IntVec3> line, IList<float> scatter,
            Func<IntVec3, bool> canImpact, Func<int, int> chooseIndex, bool allowDuplicateAims = false)
        {
            var reserved = new HashSet<IntVec3>(line);
            var used = new HashSet<IntVec3>();
            var impacts = new List<IntVec3>();
            for (int i = 0; i < line.Count; i++)
            {
                IntVec3 aim = line[i];
                // Preserve every shot's fallback square, so scatter cannot steal
                // another shot's only valid location near roofs or map edges.
                List<IntVec3> choices = Candidates(line, i, scatter[i], canImpact)
                    .Where(cell => (!used.Contains(cell) || (allowDuplicateAims && cell.Equals(aim))) &&
                        (cell.Equals(aim) || !reserved.Contains(cell))).ToList();
                if (choices.Count == 0)
                    continue;
                IntVec3 chosen = choices[chooseIndex(choices.Count)];
                used.Add(chosen);
                impacts.Add(chosen);
            }
            // Uneven spacing and lateral offsets are allowed; travel never reverses.
            return impacts.OrderBy(cell => Progress(cell, line)).ToList();
        }
    }
}
