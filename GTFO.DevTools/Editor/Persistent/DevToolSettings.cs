using System.IO;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Persistent
{
    public class DevToolSettings : PersistentObject<DevToolSettings>
    {
        public string m_rundownPath;
        public string m_authorInitials;
        public bool m_showMarkers;
        public bool m_showPlugs;
        public bool m_showGates;
        public bool m_showGeoBounds;

        static DevToolSettings()
        {
            s_path = "Assets/GTFO.DevTools/Settings.asset";
        }
    }
}
