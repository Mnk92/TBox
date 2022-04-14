using System;
using System.IO;

namespace Mnk.TBox.Plugins.Market.Client.Code
{
    static class Constants
    {
        public static readonly string PluginExtension = ".dll";
        public static readonly string PluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Plugins");
        public static readonly string DependenciesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Libraries");
    }
}
