using System;
using System.Linq;
using Mnk.Library.WpfControls;
using Mnk.TBox.Plugins.Market.Client.ServiceReference;
using Mnk.TBox.Plugins.Market.Client.Code;
using Mnk.TBox.Plugins.Market.Client.Components.Installers;

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

		public void ReloadTable(IMarketService service)
		{
			var plugin = new Plugin();
			if (!string.IsNullOrWhiteSpace(TypeName))
			{
				plugin.Type = TypeName;
			}
			if (!string.IsNullOrWhiteSpace(AuthorName))
			{
				plugin.Author = AuthorName;
			}
			originalItems = service.Plugin_GetList(plugin, 0, int.MaxValue, false);
			UpdateItems();
		}

		public void OnReloadData(IMarketService service)
		{
			Mt.Do(this, () =>{
					Types = new[] { string.Empty }.Union(Synchronizer.Types).ToArray();
					Authors = new[] { string.Empty }.Union(Synchronizer.Authors).ToArray();
					ReloadTable(service);
				});
		}

		private void UpdateItems()
		{
			if (validator!=null)
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
