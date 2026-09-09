using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Vehicles;
using Vehicles.World;
using Verse;
using Verse.Sound;

namespace JumbosJets
{
    public class CompSortie : ThingComp
    {
        private Map strikeMap;
        private List<IntVec3> strikeCells;
        private IntVec3 lastFiredCell = IntVec3.Invalid;
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
            if (SteelAvailable < Props.steelPerCell)
                return "JJ_SortieNoSteel".Translate(Props.steelPerCell);
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
            IntVec3 start = IntVec3.Invalid;
            var parameters = new TargetingParameters
            {
                canTargetLocations = true,
                canTargetPawns = false,
                canTargetBuildings = false,
                validator = target => Find.CurrentMap == map && target.Cell.InBounds(map)
            };
            Find.Targeter.BeginTargeting(parameters, target =>
            {
                string reason = AttackDisabledReason(aircraft, mission);
                List<IntVec3> cells = PreviewCells(start, target.Cell, map);
                if (reason != null || Find.CurrentMap != map || cells.Count == 0)
                {
                    Messages.Message(reason ?? "JJ_SortieInvalidTarget".Translate(), MessageTypeDefOf.RejectInput);
                    return;
                }
                strikeMap = map;
                strikeCells = StrafeArea.PlanImpacts(cells, ScatterFor(cells, map),
                    cell => ValidImpact(cell, map), count => Rand.Range(0, count), allowDuplicateAims: true);
                lastFiredCell = IntVec3.Invalid;
                nextImpact = 0;
                nextImpactTick = Find.TickManager.TicksGame + Props.approachTicks;
                readyTick = nextImpactTick + (strikeCells.Count - 1) * Props.ticksBetweenImpacts + Props.cooldownTicks;
                Messages.Message("JJ_SortieInbound".Translate(Vehicle.LabelShort),
                    new TargetInfo(cells[0], map), MessageTypeDefOf.NeutralEvent);
            }, target => DrawPreview(start, target.Cell, map), target =>
            {
                if (AttackDisabledReason(aircraft, mission) != null || Find.CurrentMap != map ||
                    !Find.Maps.Contains(map) || !target.Cell.InBounds(map))
                    return false;
                if (!start.IsValid)
                {
                    if (ValidImpact(target.Cell, map))
                        start = target.Cell;
                    // Keep this targeter active for the endpoint. Starting a new
                    // targeter in the callback would be cleared by StopTargeting.
                    return false;
                }
                return PreviewCells(start, target.Cell, map).Count > 0;
            }, onGuiAction: target =>
            {
                string label = "JJ_SortieChooseStart".Translate();
                if (start.IsValid)
                {
                    int count = PreviewCells(start, target.Cell, map).Count;
                    label = "JJ_SortieChooseEnd".Translate(count, count * Props.steelPerCell, Props.maxRunCells);
                }
                Widgets.MouseAttachedLabel(label);
            });
        }

        private static bool ValidImpact(IntVec3 cell, Map map)
        {
            return Find.Maps.Contains(map) && cell.InBounds(map) &&
                !cell.Fogged(map) && !cell.Roofed(map);
        }

        private int SteelAvailable => StrafeAmmo.Count(Vehicle.inventory?.innerContainer);

        // Future ground guidance can override/patch this hook per map and aim
        // square. Lower values tighten the footprint; zero is a precise pass.
        public virtual float GetScatterRadius(Map map, IntVec3 aimCell)
        {
            return Props.scatterRadius;
        }

        private List<float> ScatterFor(List<IntVec3> cells, Map map)
        {
            return cells.Select(cell => Mathf.Max(0f, GetScatterRadius(map, cell))).ToList();
        }

        private List<IntVec3> PreviewCells(IntVec3 start, IntVec3 end, Map map)
        {
            // Clip the geometry first: roofs/fog never extend the allowed range.
            List<IntVec3> aimedLine = StrafeLine.Cells(start, end, Props.maxRunCells)
                .Where(cell => ValidImpact(cell, map)).ToList();
            if (aimedLine.Count == 0)
                return aimedLine;
            // A-10 ammunition is delivered as a dense burst: two impacts per
            // aim square on a full-length run, capped proportionally for
            // shorter runs, by maxImpacts and by available steel.
            int impactLimit = StrafeLine.ImpactLimit(Props.maxImpacts, Props.maxRunCells, aimedLine.Count);
            var burst = new List<IntVec3>(impactLimit);
            for (int i = 0; i < impactLimit; i++)
                burst.Add(aimedLine[i * aimedLine.Count / impactLimit]);
            return burst.Take(SteelAvailable / Props.steelPerCell).ToList();
        }

        private void DrawPreview(IntVec3 start, IntVec3 end, Map map)
        {
            if (Find.CurrentMap != map || !end.InBounds(map))
                return;
            if (!start.IsValid)
            {
                start = end;
            }
            List<IntVec3> cells = PreviewCells(start, end, map);
            List<IntVec3> area = StrafeArea.Footprint(cells, ScatterFor(cells, map), Props.explosionRadius,
                cell => ValidImpact(cell, map), cell => cell.InBounds(map));
            // One union of the possible blast cells, with no internal circle outlines.
            GenDraw.DrawFieldEdges(area);
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
            if (strikeCells == null || nextImpact < 0 || nextImpact >= strikeCells.Count)
            {
                CancelStrike();
                return;
            }
            if (mission == null || !mission.OnStation ||
                mission.TargetMap != strikeMap || !HasPilot || !Vehicle.HasEnoughOperators ||
                Vehicle.CompFueledTravel == null || Vehicle.CompFueledTravel.EmptyTank)
            {
                CancelStrike();
                return;
            }
            if (Find.TickManager.TicksGame < nextImpactTick)
                return;

            IntVec3 cell = strikeCells[nextImpact];
            nextImpact++;
            nextImpactTick += Props.ticksBetweenImpacts;
            if (ValidImpact(cell, strikeMap))
            {
                if (!StrafeAmmo.TryConsume(Vehicle.inventory?.innerContainer, Props.steelPerCell))
                {
                    Messages.Message("JJ_SortieOutOfSteel".Translate(Vehicle.LabelShort), aircraft,
                        MessageTypeDefOf.CautionInput);
                    FinishStrike();
                    return;
                }
                lastFiredCell = cell;
                Props.firingSound?.PlayOneShotOnCamera(strikeMap);
                GenExplosion.DoExplosion(cell, strikeMap, Props.explosionRadius, DamageDefOf.Bomb,
                    Vehicle, Props.damageAmount, Props.armorPenetration, damageFalloff: true, doSoundEffects: false);
            }
            if (nextImpact >= strikeCells.Count)
                FinishStrike();
        }

        private void FinishStrike()
        {
            // One-shots remain alive after the comp clears the run, so firing
            // samples and the post-pass tail overlap without stopping each other.
            if (lastFiredCell.IsValid && strikeMap != null && Find.Maps.Contains(strikeMap))
                Props.afterSound?.PlayOneShotOnCamera(strikeMap);
            CancelStrike();
        }

        public void CancelStrike()
        {
            strikeMap = null;
            strikeCells = null;
            lastFiredCell = IntVec3.Invalid;
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
            Scribe_Collections.Look(ref strikeCells, "sortieStrikeCells", LookMode.Value);
            Scribe_Values.Look(ref lastFiredCell, "sortieLastFiredCell", IntVec3.Invalid);
            Scribe_Values.Look(ref nextImpact, "sortieNextImpact");
            Scribe_Values.Look(ref nextImpactTick, "sortieNextImpactTick");
            Scribe_Values.Look(ref readyTick, "sortieReadyTick");
            if (Scribe.mode == LoadSaveMode.PostLoadInit && strikeMap != null &&
                (strikeCells == null || strikeCells.Count == 0))
                CancelStrike(); // An older save may contain the former centered burst.
        }
    }
}
