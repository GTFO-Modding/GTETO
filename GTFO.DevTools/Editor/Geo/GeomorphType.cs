namespace GTFO.DevTools.Geo
{
    public enum GeomorphType
    {
        None = 0,
        Mining = 1,
        Tech = 2,
        Service = 3,
        ElevatorShaft = 4,
    }

    public static class GeoTypeExtensions
    {
        public static bool IsFloorTransition(this GeomorphType type)
        {
            return type == GeomorphType.ElevatorShaft;
        }
    }
}
