using System.Collections;
using System.Windows;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Plugins.Market.Client.Code;
using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Client.Components.Installers
{
    /// <summary>
    /// Interaction logic for AllPlugins.xaml
    /// </summary>
    partial class Installer
    {
        public class DataItem : CheckableData
        {
            public Plugin Plugin { get; set; }
        }

        protected readonly CheckableDataCollection<DataItem> Collection = new();

        protected PluginsInstaller BaseInstaller = new();
        public Installer()
        {
            InitializeComponent();
            clbItems.ItemsSource = Collection;
            clbItemsButtons.View = clbItems;
            clbItems.OnCheckChanged += clbItems_OnCheckChanged;
            cbType.SelectedIndexChanged += cbType_SelectedIndexChanged;
            cbAuthor.SelectedIndexChanged += cbAuthor_SelectedIndexChanged;
        }

        protected virtual void clbItems_OnCheckChanged(object sender, RoutedEventArgs e)
        {
            btnAction.IsEnabled = Collection.CheckedValuesCount > 0;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            DialogsCache.ShowProgress(u => Do((o, arg) => Synchronizer.RefreshTables(u)), "", null);
        }

        public Plugin[] Items
        {
            get
            {
                var checkedList = Collection.GetCheckedIndexes();
                var ret = new Plugin[checkedList.Length];
                for (var i = 0; i < checkedList.Length; ++i)
                {
                    ret[i] = BaseInstaller.Items[checkedList[i]];
                }
                return ret;
            }

            set
            {
                BaseInstaller.Items = value;
                if (value == null) return;
                var id = clbItems.SelectedIndex;
                Collection.Clear();
                foreach (var item in value)
                {
                    Collection.Add(
                        new DataItem
                        {
                            Key = item.Name,
                            IsChecked = false,
                            Plugin = item
                        }
                        );
                }
                clbItems.SelectedIndex = id;
            }
        }

        public string ActionCaption
        {
            get => btnAction.Content.ToString();
            set => btnAction.Content = value;
        }

        protected virtual void EnableControls(bool state)
        {
            Mt.Do(this, () =>
            {
                btnRefresh.IsEnabled = state;
                btnAction.IsEnabled = state;
                clbItems.IsEnabled = state;
            });
        }

        protected void Do(EventHandler action)
        {
            try
            {
                EnableControls(false);
                action(this, null);
            }
            finally
            {
                EnableControls(true);
            }
        }

        public event EventHandler OnAction;

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (OnAction != null)
            {
                Do(OnAction);
            }
        }

        public IEnumerable Authors
        {
            set => cbAuthor.ItemsSource = value;
        }

        public IEnumerable Types
        {
            set => cbType.ItemsSource = value;
        }

        public string TypeName => cbType.Value;

        public string AuthorName => cbAuthor.Value;

        public event EventHandler OnNameSelectionChanged;

        protected void cbType_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            OnNameSelectionChanged?.Invoke(this, null);
        }

        private void cbAuthor_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            OnNameSelectionChanged?.Invoke(this, null);
        }

    }
}
