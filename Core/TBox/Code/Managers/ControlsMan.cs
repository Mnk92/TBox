using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code.Objects;
using Mnk.Library.WpfControls.Components.ButtonsView;
using Mnk.Library.WpfControls.Icons;

namespace Mnk.TBox.Core.Application.Code.Managers
{
    sealed class ControlsMan
    {
        private static readonly ILog Log = LogManager.GetLogger<ControlsMan>();
        private static readonly ILog InfoLog = LogManager.GetInfoLogger<ControlsMan>();
        private readonly Action<EnginePluginInfo> onPluginChanged;
        private readonly GroupedList view;
        private readonly ContentControl owner;
        private readonly Button btnBack;
        private readonly object locker = new object();
        public ObservableCollection<IButtonInfo> Items { get; set; }

        public ControlsMan(GroupedList view, ContentControl owner, Button btnBack, Action<EnginePluginInfo> onPluginChanged)
        {
            this.view = view;
            this.owner = owner;
            this.btnBack = btnBack;
            this.onPluginChanged = onPluginChanged;
            Items = new ObservableCollection<IButtonInfo>();
        }

        private IButtonInfo FindItemByName(string key)
        {
            return Items.FirstOrDefault(x => string.Equals(x.Name, key));
        }

        private IButtonInfo FindItemByKey(string key)
        {
            return Items.Cast<ExtButtonInfo>().FirstOrDefault(x => string.Equals(x.Key, key));
        }

        public void Add(EnginePluginInfo info)
        {
            Mt.Do(view, () => DoAdd(info));
        }

        private void DoAdd(EnginePluginInfo info)
        {
            lock (locker)
            {
                var groupName = GetName(info);
                var button = FindItemByName(info.Key);
                if (button != null)
                {
                    Log.Write("Duplicate item: {0}!", info.Key);
                    return;
                }
                Items.Add(new ExtButtonInfo
                {
                    Key = info.Key,
                    Name = info.Name,
                    Icon = info.ImageSource,
                    Handler = (o, e) => OnSelectPlugin(info),
                    GroupName = groupName
                });
            }
        }

        private static string GetName(EnginePluginInfo info)
        {
            var groupName = info is EngineSettingsInfo ? "AppName" : info.PluginGroup.ToString();
            groupName = TBoxLang.ResourceManager.GetString(groupName);
            return groupName;
        }

        private void OnSelectPlugin(EnginePluginInfo info)
        {
            owner.Content = null;
            var time = Environment.TickCount;
            owner.Content = info.Settings();
            InfoLog.Write("Open settings for '{0}', time: {1}", info.Key, Environment.TickCount - time);
            btnBack.IsEnabled = true;
            onPluginChanged(info);
        }

        public void Remove(string key)
        {
            lock (locker)
            {
                var x = FindItemByKey(key);
                Items.Remove(x);
            }
        }

        public void Refresh()
        {
            view.DataContext = this;
            view.Refresh();
        }

        public void GoTo(string name)
        {
            var item = FindItemByName(name);
            if (item != null) item.Handler(null, null);
        }
    }
}
