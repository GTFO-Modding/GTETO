using GameData;

namespace GTFO.DevTools.Extensions
{
    public static class IMarkerDataBlockExtensions
    {
        public static MarkerDataCommon GetCommonData(this IMarkerDataBlock datablock)
        {
            if (datablock is MiningMarkerDataBlock mining)
                return mining.CommonData;
            else if (datablock is ServiceMarkerDataBlock service)
                return service.CommonData;
            else if (datablock is TechMarkerDataBlock tech)
                return tech.CommonData;
            else
                return null;
        }
    }
}
