using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Vehicles;
using Vehicles.World;
using Verse;

namespace JumbosJets
{
    // The framework owns the aircraft, crew, travel fuel and landing lifecycle.
    // This arrival action keeps a normal flight path open after reaching the map.
    public class ArrivalAction_Sortie : VehicleArrivalAction
    {
        private Map targetMap;
        private PlanetTile sortieTile;
        private bool onStation;
        private bool warnedLowFuel;
        private bool warnedMissingMap;

        public Map TargetMap => targetMap != null && Find.Maps.Contains(targetMap) ? targetMap : null;
        public bool OnStation => onStation;

        public ArrivalAction_Sortie() { }

        public ArrivalAction_Sortie(VehiclePawn vehicle, Map map) : base(vehicle)
        {
            targetMap = map;
            sortieTile = map.Tile;
        }

        public override void Arrived(GlobalTargetInfo target)
        {
            onStation = true;
            AerialVehicle.OrderFlyToTiles(OrbitNodes(), this);
        }

        private List<FlightNode> OrbitNodes()
        {
            var neighbors = new List<PlanetTile>();
            Find.WorldGrid.GetTileNeighbors(sortieTile, neighbors);
            var nodes = new List<FlightNode>();
            foreach (PlanetTile tile in neighbors)
                nodes.Add(new FlightNode(tile));
            nodes.Add(new FlightNode(sortieTile));
            return nodes;
        }

        public void ContinueOrbit(AerialVehicleInFlight aircraft)
        {
            // Extend before the last node is consumed. This avoids grounding,
            // repeated arrival notifications, and recon's automatic map generation.
            if (!onStation || aircraft.flightPath.Count > 1)
                return;
            foreach (FlightNode node in OrbitNodes())
                aircraft.flightPath.AddNode(node.Tile);
        }

        public void CheckWarnings(AerialVehicleInFlight aircraft)
        {
            if (TargetMap == null && !warnedMissingMap)
            {
                warnedMissingMap = true;
                Messages.Message("JJ_SortieMapGone".Translate(vehicle.LabelShort), aircraft,
                    MessageTypeDefOf.CautionInput);
            }

            CompSortie sortie = vehicle.TryGetComp<CompSortie>();
            CompFueledTravel fuel = vehicle.CompFueledTravel;
            if (!warnedLowFuel && sortie != null && fuel != null &&
                fuel.Fuel <= fuel.FuelCapacity * sortie.Props.lowFuelFraction)
            {
                warnedLowFuel = true;
                Messages.Message("JJ_SortieLowFuel".Translate(vehicle.LabelShort), aircraft,
                    MessageTypeDefOf.CautionInput);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref targetMap, "targetMap");
            Scribe_Values.Look(ref sortieTile, "sortieTile");
            Scribe_Values.Look(ref onStation, "onStation");
            Scribe_Values.Look(ref warnedLowFuel, "warnedLowFuel");
            Scribe_Values.Look(ref warnedMissingMap, "warnedMissingMap");
        }
    }
}
