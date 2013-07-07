using System;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Core.Application.Code.FastStart.Settings
{
	[Serializable]
	public class MenuItemsSequence : CheckableData
	{
		public CheckableDataCollection<CheckableData> MenuItems { get; set; }

		public MenuItemsSequence()
		{
			MenuItems = new CheckableDataCollection<CheckableData>();
		}

		public override object Clone()
		{
			return new MenuItemsSequence
				{
					IsChecked = IsChecked,
					Key = Key,
					MenuItems = MenuItems.Clone()
				};
		}
	}
}
