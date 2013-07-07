using System;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.AppConfigManager.Code
{
	[Serializable]
	public class Option : CheckableData
	{
		public string Value { get; set; }

		public override object Clone()
		{
			return new Option
			{
				IsChecked = IsChecked,
				Key = Key,
				Value = Value
			};
		}
	}
}
