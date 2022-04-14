using Mnk.Library.WpfControls;
using Mnk.TBox.Plugins.Market.Client.Code;
using Mnk.TBox.Plugins.Market.Client.Components.Installers;
using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Client.Components.Uploaders
{
    public class DependencyChooser : Installer
    {
        public DependencyChooser()
        {
            InitializeComponent();
            ActionCaption = "Choose";
            Synchronizer.OnReloadData += OnReloadData;
            OnNameSelectionChanged += DoOnNameSelectionChanged;
        }

        private void DoOnNameSelectionChanged(object sender, EventArgs e)
        {
            Synchronizer.Do(ReloadTable);
        }

        private Plugin[] originalItems;
        private Func<string, bool> validator;

        public void ReloadTable(MarketService.MarketServiceClient client)
        {
            originalItems = client.GetPluginsList(new PluginSearch
            {
                Type = TypeName,
                Author = AuthorName,
                Offset = 0,
                Count = int.MaxValue,
                OnlyPlugins = false
            }).Items.ToArray();
            UpdateItems();
        }

        public void OnReloadData(MarketService.MarketServiceClient client)
        {
            Mt.Do(this, () =>
            {
                Types = new[] { string.Empty }.Union(Synchronizer.Types).ToArray();
                Authors = new[] { string.Empty }.Union(Synchronizer.Authors).ToArray();
                ReloadTable(client);
            });
        }

        private void UpdateItems()
        {
            if (validator != null)
            {
                Items = originalItems.Where(x => validator(DependenciesSelector.FormatName(x))).ToArray();
            }
        }

        public void SetFilter(Func<string, bool> validatorFunc)
        {
            validator = validatorFunc;
            UpdateItems();
        }
    }
}
