using System.Collections.Generic;
using HarmonyLib;
using RimWorld.Planet;
using Vehicles;
using Vehicles.World;
using Verse;

namespace JumbosJets
{
    [StaticConstructorOnStartup]
    public static class SortiePatches
    {
        static SortiePatches()
        {
            new Harmony("walkinglibrary.jumbosjets.sorties").PatchAll();
        }

        [HarmonyPatch(typeof(LaunchProtocol), nameof(LaunchProtocol.GetArrivalOptions))]
        private static class ArrivalOptions
        {
            private static IEnumerable<ArrivalOption> Postfix(IEnumerable<ArrivalOption> __result,
                LaunchProtocol __instance, GlobalTargetInfo target)
            {
                foreach (ArrivalOption option in __result)
                    yield return option;
                VehiclePawn vehicle = __instance.Vehicle;
                CompSortie sortie = vehicle?.TryGetComp<CompSortie>();
                if (sortie == null || !target.IsValid)
                    yield break;
                MapParent mapParent = target.WorldObject as MapParent ?? Find.WorldObjects.MapParentAt(target.Tile);
                if (vehicle.Faction == RimWorld.Faction.OfPlayer && sortie.HasPilot &&
                    vehicle.HasEnoughOperators && mapParent != null && mapParent.HasMap &&
                    mapParent.Spawned && !mapParent.EnterCooldownBlocksEntering())
                {
                    yield return new ArrivalOption("JJ_SortieOrder".Translate(mapParent.Label),
                        new ArrivalAction_Sortie(vehicle, mapParent.Map));
                }
            }
        }

        [HarmonyPatch(typeof(AerialVehicleInFlight), nameof(AerialVehicleInFlight.GetGizmos))]
        private static class WorldGizmos
        {
            private static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, AerialVehicleInFlight __instance)
            {
                foreach (Gizmo gizmo in __result)
                    yield return gizmo;
                if (__instance.Vehicle == null || !__instance.IsPlayerControlled)
                    yield break;
                CompSortie sortie = __instance.Vehicle.TryGetComp<CompSortie>();
                var mission = __instance.flightPath.ArrivalAction as ArrivalAction_Sortie;
                if (sortie != null && mission != null)
                    foreach (Gizmo gizmo in sortie.WorldGizmos(__instance, mission))
                        yield return gizmo;
            }
        }

        [HarmonyPatch(typeof(AerialVehicleInFlight), nameof(AerialVehicleInFlight.MoveForward))]
        private static class ContinueOrbit
        {
            private static void Prefix(AerialVehicleInFlight __instance)
            {
                var mission = __instance.flightPath.ArrivalAction as ArrivalAction_Sortie;
                if (__instance.Flying && mission != null)
                    mission.ContinueOrbit(__instance);
            }
        }

        [HarmonyPatch(typeof(AerialVehicleInFlight), "Tick")]
        private static class Tick
        {
            private static void Postfix(AerialVehicleInFlight __instance)
            {
                if (!__instance.Destroyed && __instance.Vehicle != null)
                    __instance.Vehicle.TryGetComp<CompSortie>()?.TickInFlight(__instance);
            }
        }

        [HarmonyPatch(typeof(AerialVehicleInFlight), nameof(AerialVehicleInFlight.OrderFlyToTiles))]
        private static class ChangeOrders
        {
            private static void Prefix(AerialVehicleInFlight __instance, IArrivalAction arrivalAction)
            {
                if (__instance.flightPath.ArrivalAction != arrivalAction)
                    __instance.Vehicle.TryGetComp<CompSortie>()?.CancelStrike();
            }
        }
    }
}
