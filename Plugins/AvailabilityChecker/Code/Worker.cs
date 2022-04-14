using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls.Icons;
using Mnk.Library.WpfControls.Menu;
using Mnk.TBox.Locales.Localization.Plugins.AvailabilityChecker;

namespace Mnk.TBox.Plugins.AvailabilityChecker.Code
{
    class Worker : IDisposable
    {
        private readonly TrayIcon icon = new();
        private readonly IconWithText iconWithText = new(background: Color.Red);
        private readonly SafeTimer timer;
        private Config config;
        private IList<string> resources;
        private volatile int lastCount = 0;

        public Worker()
        {
            timer = new SafeTimer(OnTimer);
        }

        public void Load(Config cfg)
        {
            config = cfg;
            OnConfigUpdated(cfg);
        }

        public void Save(Config cfg, bool autoSaveOnExit)
        {
            if (autoSaveOnExit) return;
            OnConfigUpdated(cfg);
        }

        private void OnConfigUpdated(Config cfg)
        {
            cfg.Started = !cfg.Started;
            resources = cfg.Items.CheckedItems.Select(x => x.Key).ToArray();
            OnStartStop();
        }

        public void OnStartStop()
        {
            lastCount = -1;
            if (config.Started)
            {
                timer.Stop();
            }
            else
            {
                iconWithText.Create(string.Empty);
                icon.Icon = iconWithText.Icon;
                icon.HoverText = AvailabilityCheckerLang.WaitForRefresh;
                timer.Start(config.CheckInterval * 1000);
            }
            icon.IsVisible = config.Started = !config.Started;
        }

        public void OnUpdateMenu(UMenuItem[] items)
        {
            icon.SetMenuItems(items, false);
        }

        private void OnTimer()
        {
            var items = resources
                .AsParallel()
                .Where(x => !IsAvailable(x))
                .ToArray();
            if (lastCount != items.Length)
            {
                iconWithText.Create(items.Any() ? items.Length.ToString() : string.Empty);
                icon.Icon = iconWithText.Icon;
                icon.HoverText = items.Any() ?
                    string.Join(Environment.NewLine, items) :
                    "All is available";
            }
            lastCount = items.Length;
        }

        private static bool IsAvailable(string url)
        {
            try
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                {
                    return Directory.Exists(url);
                }
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(20);
                using var response = client.Send(new HttpRequestMessage(HttpMethod.Head, new Uri(url)));
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            timer.Dispose();
            icon.Dispose();
            iconWithText.Dispose();
        }
    }
}
