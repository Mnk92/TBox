using System;
using System.IO;

namespace Mnk.TBox.Plugins.Market.Client.Code
{
    static class Constants
    {
        public static string Ext = ".dll";
        public static string PluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Plugins");
        public static string DependeciesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Libraries");
    }
}
