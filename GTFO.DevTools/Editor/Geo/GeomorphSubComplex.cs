using Expedition;

namespace GTFO.DevTools.Geo
{
    public enum GeomorphSubComplex
    {
        DigSite,
        Refinery,
        Storage,
        Lab,
        DataCenter,
        Floodways
    }

    public static class GeomorphSubComplexExtensions
    {
        public static SubComplex ToSubComplex(this GeomorphSubComplex subcomplex)
        {
            switch (subcomplex)
            {
                case GeomorphSubComplex.DigSite:
                    return SubComplex.DigSite;
                case GeomorphSubComplex.Refinery:
                    return SubComplex.Refinery;
                case GeomorphSubComplex.Storage:
                    return SubComplex.Storage;
                case GeomorphSubComplex.Lab:
                    return SubComplex.Lab;
                case GeomorphSubComplex.DataCenter:
                    return SubComplex.DataCenter;
                case GeomorphSubComplex.Floodways:
                    return SubComplex.Floodways;
                default:
                    return SubComplex.All;
            }
        }
    }
}