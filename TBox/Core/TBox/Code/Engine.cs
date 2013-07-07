using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Mnk.Library.CodePlex;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Plugins;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using LightInject;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code.AutoUpdate;
using Mnk.TBox.Core.Application.Code.Configs;
using Mnk.TBox.Core.Application.Code.Managers;
using Mnk.TBox.Core.Application.Code.Objects;
using Mnk.Library.WpfControls.Localization;
using Mnk.Library.WpfWinForms.Icons;
using IUpdater = Mnk.Library.Common.MT.IUpdater;

namespace Mnk.TBox.Core.Application.Code
{
    class Engine : IDisposable
    {
        private static readonly ILog InfoLog = LogManager.GetInfoLogger<Engine>();
        private static readonly ILog Log = LogManager.GetLogger<Engine>();
        private readonly UiConfigurator uiConfigurator;
        private readonly PluginsMan plugMan;
        private readonly string pluginsReadOnlyDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private readonly string pluginsStoreDataFolder;
        private readonly string toolsDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools");
        private readonly IAutoUpdater appUpdater;
        private readonly PluginsContextShared pluginsContextShared;
        private readonly IServiceFactory container;
        private readonly ConfigManager configManager;

        public Engine(Window owner, IUpdater updater, UiConfigurator uiConfigurator, IServiceFactory container)
        {
            var time = Environment.TickCount;
            this.container = container;
            var rootFolder = container.GetInstance<IConfigsManager>().Root;
            pluginsStoreDataFolder = Path.Combine(rootFolder, "Data");
            configManager = (ConfigManager)container.GetInstance<IConfigManager<Config>>();
            this.uiConfigurator = uiConfigurator;
            InfoLog.Write("Load first config time: {0}", Environment.TickCount - time);
            updater.Update(TBoxLang.CheckForUpdates, 0.06f);
            appUpdater = container.GetInstance<IAutoUpdater>();
            ThreadPool.QueueUserWorkItem(o => appUpdater.TryUpdate());
            updater.Update(TBoxLang.Prepare, 0.07f);
            plugMan = new PluginsMan(
                new Factory<IPlugin>(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libraries")
                    ),
                Path.Combine(rootFolder, "Config"),
                a => Mt.Do(owner, a)
                );
            var toAdd = new List<string>();
            pluginsContextShared = container.GetInstance<PluginsContextShared>();
            pluginsContextShared.Sync = uiConfigurator.Sync;
            uiConfigurator.Init(owner, configManager.Config, appUpdater, SetupPlugins(toAdd));
            AddPlugins(toAdd, updater);
            uiConfigurator.ShowTrayIcon(owner);
        }

        private IEnumerable<EnginePluginInfo> SetupPlugins(ICollection<string> toAdd)
        {
            var names = plugMan.Factory.Names.OrderBy(x => x).ToArray();
            foreach (var name in names)
            {
                PluginInfoAttribute a = null;
                try
                {
                    var t = plugMan.Factory.Get(name);
                    a = GetAttribute<PluginInfoAttribute>(t);
                }
                catch (Exception ex)
                {
                    Log.Write(ex, "Plugin initialization error. Can't get valid plugin info attribute from plugin: " + name);
                    plugMan.Factory.Remove(name);
                }
                if (a == null) continue;
                var icon = a.IsIconSystem ? pluginsContextShared.GetSystemIcon(a.SystemIconId) : a.Icon;
                var enabled = !configManager.Config.DisabledItems.Contains(name);
                yield return new EnginePluginInfo(name, a.Name, a.Description, icon, enabled, a.PluginGroup);
                if (enabled)
                {
                    toAdd.Add(name);
                }
            }
        }

        private static T GetAttribute<T>(Type t) where T : Attribute
        {
            var attributes = t.GetCustomAttributes(typeof(T), false);
            if (!attributes.Any()) return null;
            var a = attributes[0] as T;
            if (a == null) throw new ArgumentException("Plugin should have valid attribute");
            return a;
        }

        private void AddPlugins(IEnumerable<string> names, IUpdater updater)
        {
            updater.Update(TBoxLang.CreatePlugins, 0.33f);
            CreatePlugins(names.ToArray(), updater);
            container.GetInstance<WarmingUpManager>().CreateAll();
        }

        private void CreatePlugins(string[] names, IUpdater updater)
        {
            var time = Environment.TickCount;
            var ret = new List<EnginePluginInfo>(names.Length);
            var culture = Translator.Culture;
            Parallel.ForEach(names, name => CreatePlugin(names, updater, name, ret, culture));
            uiConfigurator.InitUi(ret);
            InfoLog.Write("Create all plugin time: {0}", Environment.TickCount - time);
        }

        private void CreatePlugin(IEnumerable<string> names, IUpdater updater, string name, List<EnginePluginInfo> ret, CultureInfo culture)
        {
            Translator.Culture = culture;
            var time = Environment.TickCount;
            var plg = Add(name);
            if (plg == null) return;
            var pmu = new PluginMenuUpdater(x => uiConfigurator.ConfigurePlugin(plg, x));
            plg.Init(
                new PluginContext(
                    new DataProvider(toolsDataFolder, Path.Combine(pluginsReadOnlyDataFolder, name), Path.Combine(pluginsStoreDataFolder, name)),
                    container.GetInstance<IPathResolver>(),
                    pmu.Do,
                    pluginsContextShared,
                    ()=>SaveConfig(name)));
            var ui = InitPlugin(name, plg);
            if (ui != null)
            {
                pmu.Init(ui.Name);
                lock (ret)
                {
                    ret.Add(ui);
                }
            }
            InfoLog.Write("Create plugin time: {1}, log : '{0}'", name, Environment.TickCount - time);
            updater.Update(TBoxLang.Create + ": " + (ui != null ? ui.Name : name), 0.3f + (0.7f * ret.Count) / names.Count());
        }

        private void HideCheck(string key)
        {
            uiConfigurator.SetPluginCheck(key, false);
            configManager.Config.DisabledItems.Add(key);
        }

        private void RemovePlugins(IEnumerable<string> names)
        {
            Mt.Do(uiConfigurator.Sync,
                () =>
                {
                    foreach (var name in names.ToArray())
                    {
                        Remove(name);
                    }
                });
        }

        public void Load(IUpdater updater)
        {
            uiConfigurator.DoHoldOperation(() => DoLoad(updater));
        }

        private void DoLoad(IUpdater updater)
        {
            configManager.Load();
            foreach (var name in plugMan.Factory.Names)
            {
                uiConfigurator.SetPluginCheck(name, !configManager.Config.DisabledItems.Contains(name));
            }
            plugMan.Load(updater);
            uiConfigurator.Load(configManager.Config);
        }

        public void Save(IUpdater updater, bool autoSaveOnExit)
        {
            uiConfigurator.DoHoldOperation(() => DoSave(updater, autoSaveOnExit));
        }

        private void DoSave(IUpdater updater, bool autoSaveOnExit)
        {
            updater.Update(TBoxLang.UpdateDisabledItems, 0);
            var disabledBefore = new List<string>(configManager.Config.DisabledItems);
            configManager.Config.DisabledItems.Clear();
            var toAdd = new List<string>();
            var toRemove = new List<string>();
            plugMan.Save(updater, autoSaveOnExit);
            foreach (var item in uiConfigurator.GetPluginsStates(toAdd, toRemove, disabledBefore, plugMan))
            {
                configManager.Config.DisabledItems.Add(item);
            }
            AddPlugins(toAdd, updater);
            RemovePlugins(toRemove);
            updater.Update(TBoxLang.Save, 1.0f);
            uiConfigurator.Save(configManager.Config);
            configManager.Save();
        }

        private void SaveConfig(string name)
        {
            plugMan.SaveItem(name);
        }

        public EnginePluginInfo InitPlugin(string key, IPlugin plugin)
        {
            try
            {
                var p = uiConfigurator.InitPlugin(plugin, key);
                plugMan.LoadItem(key);
                return p;
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Error init: '{0}'", key);
                Remove(key);
                HideCheck(key);
            }
            return null;
        }

        private IPlugin Add(string key)
        {
            IPlugin plg = null;
            try
            {
                plg = plugMan.Create(key);
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't create plugin: '{0}'", key);
            }
            if (plg == null)
            {
                HideCheck(key);
            }
            return plg;
        }

        private void Remove(string key)
        {
            uiConfigurator.Remove(key);
            plugMan.Remove(key);
        }

        public bool Close(IUpdater updater, bool criticalError)
        {
            if (!criticalError)
            {
                plugMan.Save(updater, true);
                configManager.Save();
            }
            RemovePlugins(plugMan.Factory.Names);
            return true;
        }

        public void Dispose()
        {
            container.GetInstance<IconsCache>().Dispose();
        }

        public void CheckUpdates(bool silent)
        {
            var result = appUpdater.TryUpdate(true);
            if (result == true) return;
            if (!silent && result!=null) MessageBox.Show(TBoxLang.MessageNoUpdatesFound, TBoxLang.AppName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}
