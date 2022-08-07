using System;
using System.Reflection;
using System.Linq;
using GameData;
using UnityEngine;

namespace GTFO.DevTools.Extensions
{
    public static class MarkerProducerExtensions
    {
        private readonly static Type[] s_MiningMarkerTypes = new Type[]
        {
            typeof(MiningDigSiteMarkerProducer),
            typeof(MiningDigSiteSubMarkerProducer),
            typeof(MiningExpeditionMarkerProducer),
            typeof(MiningRefineryMarkerProducer),
            typeof(MiningRefinerySubMarkerProducer),
            typeof(MiningStorageMarkerProducer),
            typeof(MiningSubMarkerProducer)
        };

        private readonly static Type[] s_TechMarkerTypes = new Type[]
        {
            typeof(TechLabMarkerProducer),
            typeof(TechDataCenterMarkerProducer),
            typeof(TechExpeditionMarkerProducer)
        };

        private readonly static Type[] s_ServiceMarkerTypes = new Type[]
        {
            typeof(ServiceFloodwaysMarkerProducer),
            typeof(ServiceFloodwaysMarkerProducer),
            typeof(ServiceGardensMarkerProducer)
        };

        private const string datablockIDField = "m_markerDataBlockID";

        public static bool TryGetMarkerBlockInfo(this LG_MarkerProducer producer, out LG_MarkerDataBlockType blockType, out uint blockID)
        {
            // set initial fail condition
            blockType = LG_MarkerDataBlockType.Mining;
            blockID = 0;

            Type type = producer.GetType();

            // assume success. This will be set to false if it could not fetch the block type.
            bool success = true;
            if (s_MiningMarkerTypes.Contains(type))
            {
                blockType = LG_MarkerDataBlockType.Mining;
            }
            else if (s_TechMarkerTypes.Contains(type))
            {
                blockType = LG_MarkerDataBlockType.Tech;
            }
            else if (s_ServiceMarkerTypes.Contains(type))
            {
                blockType = LG_MarkerDataBlockType.Service;
            }
            else
            {
                success = false;
            }

            if (success)
            {
                // fetch the datablock id from the producer
                blockID = (uint)type.GetField(datablockIDField).GetValue(producer);
            }

            return success;
        }
    }
}