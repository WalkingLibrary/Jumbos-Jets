using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Vehicles;
using Vehicles.World;
using Verse;

namespace JumbosJets
{
    public class CompSortie : ThingComp
    {
        private Map strikeMap;
        private IntVec3 strikeCenter = IntVec3.Invalid;
        private int nextImpact;
        private int nextImpactTick;
        private int readyTick;

        public CompProperties_Sortie Props => (CompProperties_Sortie)props;
        private VehiclePawn Vehicle => (VehiclePawn)parent;

        public bool HasPilot => Vehicle.GetHandlers(HandlingType.Movement)
            .Any(handler => handler.role.slotsToOperate > 0 && handler.RoleFulfilled);

        private string AttackDisabledReason(AerialVehicleInFlight aircraft, ArrivalAction_Sortie mission)
        {
            if (!SortieFlight.IsFlying(aircraft) || aircraft.flightPath.ArrivalAction != mission)
                return "JJ_SortieUnavailable".Translate();
            if (!mission.OnStation)
                return "JJ_SortieEnRoute".Translate();
            if (mission.TargetMap == null)
                return "JJ_SortieNoMap".Translate();
            if (!HasPilot || !Vehicle.HasEnoughOperators)
                return "JJ_SortieNeedsPilot".Translate();
            if (Vehicle.CompFueledTravel == null || Vehicle.CompFueledTravel.EmptyTank)
                return "JJ_SortieNoFuel".Translate();
            if (strikeMap != null || Find.TickManager.TicksGame < readyTick)
                return "JJ_SortieRearming".Translate();
            return null;
        }

        public IEnumerable<Gizmo> WorldGizmos(AerialVehicleInFlight aircraft, ArrivalAction_Sortie mission)
        {
            var attack = new Command_Action
            {
                defaultLabel = "JJ_SortieAttack".Translate(),
                defaultDesc = "JJ_SortieAttackDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack"),
                action = () => BeginAttack(aircraft, mission)
            };
            string reason = AttackDisabledReason(aircraft, mission);
            if (reason != null)
                attack.Disable(reason);
            yield return attack;

            var land = new Command_Action
            {
                defaultLabel = "JJ_SortieLand".Translate(),
                defaultDesc = "JJ_SortieLandDesc".Translate(),
                icon = TexCommand.Install,
                action = () => BeginLanding(aircraft, mission)
            };
            if (mission.TargetMap == null)
                land.Disable("JJ_SortieNoMap".Translate());
            else if (mission.TargetMap.Parent.EnterCooldownBlocksEntering())
                land.Disable("JJ_SortieLandingCooldown".Translate());
            yield return land;
        }

        private void BeginLanding(AerialVehicleInFlight aircraft, ArrivalAction_Sortie mission)
        {
            Map map = mission.TargetMap;
            if (map == null || !SortieFlight.IsFlying(aircraft) || map.Parent.EnterCooldownBlocksEntering())
                return;
            Current.Game.CurrentMap = map;
            CameraJumper.TryHideWorld();
            LandingTargeter.Instance.BeginTargeting(Vehicle, map, (target, rotation) =>
            {
                if (!SortieFlight.IsFlying(aircraft) || mission.TargetMap != map ||
                    aircraft.flightPath.ArrivalAction != mission || map.Parent.EnterCooldownBlocksEntering())
                    return;
                CancelStrike();
                aircraft.OrderFlyToTiles(new List<FlightNode> { new FlightNode(map.Tile) },
                    new ArrivalAction_LandToCell(Vehicle, map.Parent, target.Cell, rotation));
                CameraJumper.TryShowWorld();
            }, target => target.Cell.InBounds(map) &&
                !Ext_Vehicles.IsRoofRestricted(Vehicle.VehicleDef, target.Cell, map),
                null, null, null, Vehicle.VehicleDef.rotatable);
        }

        private void BeginAttack(AerialVehicleInFlight aircraft, ArrivalAction_Sortie mission)
        {
            if (AttackDisabledReason(aircraft, mission) != null)
                return;
            Map map = mission.TargetMap;
            Current.Game.CurrentMap = map;
            CameraJumper.TryHideWorld();
            Find.Selector.ClearSelection();
            var parameters = new TargetingParameters
            {
                canTargetLocations = true,
                canTargetPawns = false,
                canTargetBuildings = false,
                validator = target => ValidTarget(target.Cell, map)
            };
            Find.Targeter.BeginTargeting(parameters, target =>
            {
                string reason = AttackDisabledReason(aircraft, mission);
                if (reason != null || !ValidTarget(target.Cell, map))
                {
                    Messages.Message(reason ?? "JJ_SortieInvalidTarget".Translate(), MessageTypeDefOf.RejectInput);
                    return;
                }
                strikeMap = map;
                strikeCenter = target.Cell;
                nextImpact = 0;
                nextImpactTick = Find.TickManager.TicksGame + Props.approachTicks;
                readyTick = nextImpactTick + Props.burstCount * Props.ticksBetweenImpacts + Props.cooldownTicks;
                Messages.Message("JJ_SortieInbound".Translate(Vehicle.LabelShort),
                    new TargetInfo(strikeCenter, map), MessageTypeDefOf.NeutralEvent);
            }, target => DrawPreview(target.Cell, map), target => ValidTarget(target.Cell, map));
        }

        private static bool ValidTarget(IntVec3 cell, Map map)
        {
            return Find.CurrentMap == map && Find.Maps.Contains(map) && cell.InBounds(map) &&
                !cell.Fogged(map) && !cell.Roofed(map);
        }

        public IntVec3 ImpactCell(IntVec3 center, int index)
        {
            return new IntVec3(center.x + (index - Props.burstCount / 2) * Props.cellSpacing, 0, center.z);
        }

        private void DrawPreview(IntVec3 center, Map map)
        {
            if (Find.CurrentMap != map || !center.InBounds(map))
                return;
            for (int i = 0; i < Props.burstCount; i++)
            {
                IntVec3 cell = ImpactCell(center, i);
                if (cell.InBounds(map) && !cell.Roofed(map))
                    GenDraw.DrawRadiusRing(cell, Props.explosionRadius);
            }
        }

        // Unspawned vehicle comps do not tick normally; the aerial world object drives this.
        public void TickInFlight(AerialVehicleInFlight aircraft)
        {
            if (!SortieFlight.IsFlying(aircraft))
            {
                CancelStrike();
                return;
            }
            var mission = aircraft.flightPath.ArrivalAction as ArrivalAction_Sortie;
            if (mission != null)
                mission.CheckWarnings(aircraft);
            if (strikeMap == null)
                return;
            if (mission == null || !mission.OnStation ||
                mission.TargetMap != strikeMap || !HasPilot || !Vehicle.HasEnoughOperators ||
                Vehicle.CompFueledTravel == null || Vehicle.CompFueledTravel.EmptyTank)
            {
                CancelStrike();
                return;
            }
            if (Find.TickManager.TicksGame < nextImpactTick)
                return;

            IntVec3 cell = ImpactCell(strikeCenter, nextImpact);
            nextImpact++;
            nextImpactTick += Props.ticksBetweenImpacts;
            if (cell.InBounds(strikeMap) && !cell.Roofed(strikeMap))
            {
                GenExplosion.DoExplosion(cell, strikeMap, Props.explosionRadius, DamageDefOf.Bomb,
                Vehicle, Props.damageAmount, Props.armorPenetration, damageFalloff: true);
            }
            if (nextImpact >= Props.burstCount)
                CancelStrike();
        }

        public void CancelStrike()
        {
            strikeMap = null;
            strikeCenter = IntVec3.Invalid;
            nextImpact = 0;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            // Landing and forced crash spawns must not retain an airborne attack.
            CancelStrike();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref strikeMap, "sortieStrikeMap");
            Scribe_Values.Look(ref strikeCenter, "sortieStrikeCenter", IntVec3.Invalid);
            Scribe_Values.Look(ref nextImpact, "sortieNextImpact");
            Scribe_Values.Look(ref nextImpactTick, "sortieNextImpactTick");
            Scribe_Values.Look(ref readyTick, "sortieReadyTick");
        }
    }
}
