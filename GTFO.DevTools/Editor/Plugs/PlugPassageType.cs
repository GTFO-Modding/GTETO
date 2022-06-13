using System;
using System.Collections.Generic;
using System.Text;

namespace GTFO.DevTools.Plugs
{
    public enum PlugPassageType
    {
        Free,
        SmallGate,
        MediumGate,
        SmallGateAlign,
        MediumGateAlign,
        Cap,
        Custom
    }

    public static class PlugPassageTypeExtensions
    {
        public static bool IsGate(this PlugPassageType type)
        {
            return type >= PlugPassageType.SmallGate && type <= PlugPassageType.MediumGate;
        }

        public static bool IsInternalGate(this PlugPassageType type)
        {
            return type >= PlugPassageType.SmallGate && type <= PlugPassageType.MediumGate;
        }

        public static bool IsGateAlign(this PlugPassageType type)
        {
            return type >= PlugPassageType.SmallGateAlign && type <= PlugPassageType.MediumGateAlign;
        }

        public static bool IsSmallGate(this PlugPassageType type)
        {
            return type == PlugPassageType.SmallGate ||
                type == PlugPassageType.SmallGateAlign;
        }

        public static bool IsMediumGate(this PlugPassageType type)
        {
            return type == PlugPassageType.MediumGate ||
                type == PlugPassageType.MediumGateAlign;
        }
    }
}
