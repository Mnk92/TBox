using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Code;
using Mnk.TBox.Plugins.Market.Client.ServiceReference;
using Mnk.Library.WpfControls.Code.Dialogs;
using Mnk.TBox.Plugins.Market.Client.Code;

namespace Mnk.TBox.Plugins.Market.Client.Components.Uploaders
{
	/// <summary>
	/// Interaction logic for PluginUploaderDialog.xaml
	/// </summary>
	public partial class PluginUploaderDialog
	{
		private static readonly ILog Log = LogManager.GetLogger<PluginUploaderDialog>();
		class FilePathData : Data
		{
			public override string ToString()
			{
				return Path.GetFileName(Key) ?? string.Empty;
			}
		}

		enum EditMode
		{
			Edit,
			Add,
		}
		private EditMode editMode = EditMode.Edit;

		private readonly ObservableCollection<FilePathData> currPlugins =
			new ObservableCollection<FilePathData>();

		private readonly ObservableCollection<Data> currDependencies =
			new ObservableCollection<Data>();

		private readonly PathTemplates pluginsTemplates = new PathTemplates(
			add: Properties.Resources.AddPath,
			del: Properties.Resources.DelPath,
			clear: Properties.Resources.ClearAllPaths,
			invalidPath: Properties.Resources.InvalidPath
		);

		private readonly Templates dependenciesTemplates = new Templates(
			add: Properties.Resources.AddDependency,
			del: Properties.Resources.DelDependency,
			clear: Properties.Resources.ClearAllDependencies
		);

		private PluginUploader baseUploader;
		public PluginUploaderDialog()
		{
			InitializeComponent();
			Synchronizer.OnReloadData += OnReloadData;
			lbPlugins.ConfigureSelector(
				currPlugins,
				ebpPlugins,
				new InputFilePath(
					"Plugins",
					pluginsTemplates,
					text => currPlugins.IsUniqueIgnoreCase(s => s.ToString(), Path.GetFileName(text)),
                    ()=>null
					)
				);
			currPlugins.CollectionChanged += PluginsChanged;

			lbDependencies.ConfigureSelector<Data>(
				currDependencies,
				ebpDependencies,
				new DependenciesSelector("Dependencies",
					dependenciesTemplates,
					x => currDependencies.IsUniqueIgnoreCase(a => a.ToString(),x),
                    () => null
					)
				);
			currDependencies.CollectionChanged += DependenciesChanged;
		}

		private void DependenciesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{

		}

		private void PluginsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RefreshData(sender, null);
		}

		public void Init()
		{
			baseUploader = new PluginUploader(CreateItem);
			rbIsAddOrEditMode_Click(null, null);
		}

		protected Plugin CreateItem()
		{
			Plugin ret = null;
			Mt.Do(this, () => ret = new Plugin
			{
				Author = cmbAuthor.Value,
				Date = DateTime.Now,
				//Dependenses = lbDependenses.Text,
				Description = tbDescription.Text,
				Name = cmbName.Value,
				Type = cmbType.Value,
				IsPlugin = rbIsPlugin.IsChecked ?? false,
			});
			return ret;
		}

		private int GetSelectedItemId()
		{
			var items = baseUploader.Items;
			if (items != null)
			{
				for (var i = 0; i < items.Length; ++i)
				{
					if (items[i].Name != cmbName.Value) continue;
					return i;
				}
			}
			return -1;
		}

		private void RefreshData(object sender, RoutedEventArgs e)
		{
			var id = GetSelectedItemId();
			if (id > -1 && editMode == EditMode.Edit)
			{
				OnNameChangedToExist(baseUploader.Items[id]);
				spInfo.Visibility = Visibility.Visible;
			}
			else
			{
				spInfo.Visibility = Visibility.Hidden;
			}
			btnDelete.IsEnabled = (id > -1) && (editMode == EditMode.Edit);
			btnUpgrade.IsEnabled = btnDelete.IsEnabled && (IsPluginNameValid() || currPlugins.Count == 0);
			UpdateButton(null, null);
		}

		protected virtual void OnNameChangedToExist(Plugin item)
		{
			tbDescription.Text = item.Description;
			lDownloads.Content = item.Downloads.ToString();
			lUploads.Content = item.Uploads.ToString();
			lDate.Content = item.Date.ToShortDateString();
			lSize.Content = item.Size / 1024 + "Kb";
			cmbType.Value = item.Type;
			if (item.IsPlugin) rbIsPlugin.IsChecked = true;
			else rbIsDependency.IsChecked = true;
		}

		protected virtual void OnReloadData(IMarketService service)
		{
			Mt.Do(this, () =>
			{
				cmbAuthor.Value = Environment.UserDomainName + "\\" + Environment.UserName;
				baseUploader.GetList(service, cmbAuthor.Value);
				cmbName.ItemsSource = baseUploader.Items.Select(x => x.Name).ToArray();
				cmbType.ItemsSource = Synchronizer.Types.ToArray();
			});
		}

		protected void UpdateButton(object sender, RoutedEventArgs e)
		{
			btnUpload.IsEnabled = (GetSelectedItemId() == -1) &&
				!IsDataNotFilled() &&
				(editMode == EditMode.Add);
		}

		protected bool IsPluginNameValid()
		{
			if (cmbName.Value.IndexOf(DependenciesSelector.Divider) != -1) return false;
			var name = (cmbName.Value + ".dll").ToUpper();
			return currPlugins.Count(x => x.ToString().ToUpper() == name) == 1;
		}

		protected virtual bool IsDataNotFilled()
		{
			return string.IsNullOrWhiteSpace(tbDescription.Text) ||
					string.IsNullOrWhiteSpace(cmbAuthor.Value) ||
					lbPlugins.Items.Count == 0 ||
					string.IsNullOrWhiteSpace(cmbType.Value) ||
					!IsPluginNameValid();
		}

		private void ActionWithButton(Control button, string description, Action work)
		{
			button.IsEnabled = false;
			try
			{
				work();
			}
			catch (Exception ex)
			{
				Log.Write(ex, "While {1} item '{0}'", cmbName.Value, description);
			}
			finally
			{
				button.IsEnabled = true;
				RefreshData(null, null);
			}
		}

		private void btnUpload_Click(object sender, RoutedEventArgs e)
		{
			ActionWithButton(btnUpload, "upload",
				() => baseUploader.Upload(cmbName.Value, currPlugins.Select(x => x.Key).ToArray())
			);
		}

		private void btnUpgrade_Click(object sender, RoutedEventArgs e)
		{
			ActionWithButton(btnUpgrade, "upgrade",
				() => baseUploader.Upgrade(cmbName.Value, currPlugins.Select(x => x.Key).ToArray())
			);
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			ActionWithButton(btnDelete, "delete",
				() => baseUploader.Delete(cmbName.Value)
			);
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			ActionWithButton(btnRefresh, "refresh", () => baseUploader.Refresh());

		}

		private void rbSetPluginOrDependency_Click(object sender, RoutedEventArgs e)
		{

		}

		private void rbIsAddOrEditMode_Click(object sender, RoutedEventArgs e)
		{
			editMode = (rbIsEditMode.IsChecked == true) ? EditMode.Edit : EditMode.Add;
			btnUpgrade.Visibility = btnDelete.Visibility =
				(editMode == EditMode.Edit) ?
					Visibility.Visible : Visibility.Collapsed;
			btnUpload.Visibility =
				(btnUpgrade.Visibility == Visibility.Collapsed) ?
					Visibility.Visible : Visibility.Collapsed;
			spPluginOrDependency.IsEnabled = rbIsEditMode.IsChecked != true;
			RefreshData(sender, e);
		}

	}
}
