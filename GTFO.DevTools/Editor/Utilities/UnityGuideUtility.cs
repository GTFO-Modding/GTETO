using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace GTFO.DevTools.Utilities
{
    internal static class UnityGuideUtility
    {
        const string URL_GUIDE = "https://docs.google.com/document/d/10z7OhZ1ggk6CfLeC3AphsEVym0YHJfchF5-v1aQnz6M/edit?usp=sharing";

        [MenuItem("Unity Guide/Open")]
        public static void OpenGuide()
        {
            // windows
            Process.Start(new ProcessStartInfo(URL_GUIDE.Replace("&", "^&")) { UseShellExecute = true });
        }
    }
}
