using System;
using System.Collections.ObjectModel;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Core.Application.Code.FastStart.Settings
{
	[Serializable]
	public class FastStartConfig
	{
		public int MaxCount { get; set; }
		public bool IsFastStart { get; set; }
		public ObservableCollection<MenuItemStatistic> MenuItems { get; set; }
		public CheckableDataCollection<MenuItemsSequence> MenuItemsSequence { get; set; }
		
		public FastStartConfig()
		{
			MaxCount = 20;
			IsFastStart = false;
			MenuItems = new ObservableCollection<MenuItemStatistic>();
			MenuItemsSequence = new CheckableDataCollection<MenuItemsSequence>();
		}

		public FastStartConfig Clone()
		{
			return new FastStartConfig
				{
					IsFastStart = IsFastStart,
					MaxCount = MaxCount,
					//MenuItems = MenuItems.Clone(),
					MenuItemsSequence = MenuItemsSequence.Clone()
				};
		}
	}
}
