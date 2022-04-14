using System.Windows.Controls;
using Mnk.TBox.Plugins.Market.Client.Code;
using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Client.Components
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History
    {
        public History()
        {
            InitializeComponent();
            Synchronizer.OnInstall += OnInstall;
            Synchronizer.OnUninstall += OnUninstall;
        }

        private DateTime lastDate = DateTime.MinValue;
        private void OnUninstall(Plugin plugin)
        {
            OnInstallOrUninstall(plugin, false);
        }

        private void OnInstall(Plugin plugin)
        {
            OnInstallOrUninstall(plugin, true);
        }

        private List<Config.History.Info> historyItems;
        private void OnInstallOrUninstall(Plugin plugin, bool installed)
        {
            var date = DateTime.Now;
            var item = new Config.History.Info
            {
                Author = plugin.Author,
                Name = plugin.Name,
                Date = new DateTime(date.Year, date.Month, date.Day),
                Installed = installed,
            };
            historyItems.Add(item);
            Add(item);
        }

        private static string FormatName(Config.History.Info info)
        {
            return $"[{(info.Installed ? "I" : "U")}] {info.Name} - {info.Author}";
        }

        private void Add(Config.History.Info item)
        {
            if (lastDate < item.Date)
            {
                tvHistory.Items.Add(new TreeViewItem { Header = item.Date.ToShortDateString() });
                lastDate = item.Date;
            }
            var last = (TreeViewItem)tvHistory.Items[tvHistory.Items.Count - 1];
            last.Items.Add(FormatName(item));
        }

        public void Fill(Config config)
        {
            if (historyItems == null)
            {
                historyItems = new List<Config.History.Info>(config.HistoryConfig.Items.
                    OrderBy(x => x.Date).ToArray());
                foreach (var item in historyItems)
                {
                    Add(item);
                }
            }
            nupMaxItemsInHistory.Value = config.HistoryConfig.MaxItemsInHistory;
        }

        public void Read(Config config)
        {
            config.HistoryConfig.MaxItemsInHistory = nupMaxItemsInHistory.Value;
            config.HistoryConfig.Items.Clear();
            if (historyItems.Count > config.HistoryConfig.MaxItemsInHistory)
            {
                var count = historyItems.Count - config.HistoryConfig.MaxItemsInHistory;
                if (count > 0)
                {
                    historyItems.RemoveRange(0, count);
                }
            }
            config.HistoryConfig.Items.AddRange(historyItems);
        }

    }
}
