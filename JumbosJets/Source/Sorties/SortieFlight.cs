using Vehicles;
using Vehicles.World;

namespace JumbosJets
{
    internal static class SortieFlight
    {
        public static bool IsValid(AerialVehicleInFlight aircraft)
        {
            if (aircraft == null || aircraft.Destroyed)
                return false;
            VehiclePawn vehicle = aircraft.Vehicle;
            return vehicle != null && !vehicle.Destroyed &&
                vehicle.CompVehicleLauncher != null && aircraft.flightPath != null;
        }

        public static bool IsFlying(AerialVehicleInFlight aircraft)
        {
            return IsValid(aircraft) && aircraft.Vehicle.CompVehicleLauncher.inFlight;
        }

        public static bool BeforeMoveForward(AerialVehicleInFlight aircraft)
        {
            // Tick can finish a crash (which clears Vehicle and the path), then
            // still call MoveForward. Skip the original too: it assumes a live flight.
            if (!IsFlying(aircraft))
                return false;
            var mission = aircraft.flightPath.ArrivalAction as ArrivalAction_Sortie;
            if (mission != null)
                mission.ContinueOrbit(aircraft);
            return true;
        }
    }
}
