using ECommons;
using ECommons.DalamudServices;
using ECommons.Reflection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Artisan.RawInformation
{
    internal static class DalamudInfo
    {
        public static bool StagingChecked = false;
        public static bool IsStaging = false;
        // TODO(api13): Svc.PluginInterface.GetDalamudVersion()/IDalamudVersionInfo is API15-only;
        // TC_ok/_dalamud_api13's IDalamudPluginInterface has no such method. B1-stubbed to "not staging".
        public static bool IsOnStaging() => false;
    }
}
