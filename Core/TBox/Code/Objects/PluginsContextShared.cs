using System;
using System.Drawing;
using System.Windows.Controls;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Application.Code.Managers;
using Mnk.Library.WpfControls.Icons;

namespace Mnk.TBox.Core.Application.Code.Objects
{
    class PluginsContextShared
    {
        private static readonly ILog Log = LogManager.GetLogger<PluginsContextShared>();
        internal Control Sync { get; set; }
        private readonly IconsCache iconsCache;
        private readonly IconsExtractor iconsExtractor;
        private readonly WarmingUpManager warmingUpManager;

        public PluginsContextShared(IconsCache iconsCache, IconsExtractor iconsExtractor, WarmingUpManager warmingUpManager)
        {
            this.iconsCache = iconsCache;
            this.warmingUpManager = warmingUpManager;
            this.iconsExtractor = iconsExtractor;
        }

        public void DoSync(Action action)
        {
            Mt.Do(Sync, action);
        }

        public void AddTypeToWarmingUp(Type t)
        {
            warmingUpManager.TryAdd(t);
        }

        public Icon GetIcon(string path, int id)
        {
            try
            {
                lock (iconsExtractor)
                {
                    var icon = iconsCache.Get(path, id);
                    if (icon == null)
                    {
                        icon = iconsExtractor.GetIcon(path, id, true);
                        iconsCache.Add(path, id, icon);
                    }
                    return icon;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't load icon: " + path);
                return null;
            }
        }

        public Icon GetSystemIcon(int id)
        {
            return GetIcon("C:/Windows/System32/shell32.dll", id);
        }
    }
}
