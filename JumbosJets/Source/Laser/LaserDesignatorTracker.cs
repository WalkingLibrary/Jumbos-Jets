using System.Collections.Generic;
using Verse;

namespace JumbosJets
{
    public static class LaserDesignatorTracker
    {
        private static readonly Dictionary<CompLaserDesignator, Map> owners = new Dictionary<CompLaserDesignator, Map>();
        private static readonly Dictionary<CompLaserDesignator, IntVec3> targets = new Dictionary<CompLaserDesignator, IntVec3>();

        public static void Set(CompLaserDesignator source, Map map, IntVec3 cell)
        {
            Clear(source);
            owners[source] = map;
            targets[source] = cell;
        }

        public static void Clear(CompLaserDesignator source)
        {
            if (source != null && owners.TryGetValue(source, out Map map))
            {
                owners.Remove(source);
                targets.Remove(source);
            }
        }

        public static bool TryGetTarget(Map map, out IntVec3 cell)
        {
            foreach (KeyValuePair<CompLaserDesignator, Map> entry in owners)
            {
                if (entry.Value == map && entry.Key.IsAiming && entry.Key.User.Map == map &&
                    targets.TryGetValue(entry.Key, out cell))
                    return true;
            }
            cell = IntVec3.Invalid;
            return false;
        }

        public static IEnumerable<KeyValuePair<CompLaserDesignator, IntVec3>> ForMap(Map map)
        {
            foreach (KeyValuePair<CompLaserDesignator, Map> entry in owners)
            {
                if (entry.Value == map && entry.Key.IsAiming && entry.Key.User.Map == map &&
                    targets.TryGetValue(entry.Key, out IntVec3 cell))
                    yield return new KeyValuePair<CompLaserDesignator, IntVec3>(entry.Key, cell);
            }
        }
    }
}
