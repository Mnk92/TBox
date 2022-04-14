using System;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Plugins.AppConfigManager.Code
{
	[Serializable]
	public class Profile : Data
	{
		public CheckableDataCollection<CheckableData> Files { get; set; }
		public CheckableDataCollection<Option> Options { get; set; }

		public Profile()
		{
			Files = new CheckableDataCollection<CheckableData>();
			Options = new CheckableDataCollection<Option>();
		}

		public override object Clone()
		{
			return new Profile
			{
				Key = Key,
				Files = Files.Clone(),
				Options = Options.Clone()
			};
		}
	}
}
