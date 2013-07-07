using System;
using System.Collections.ObjectModel;

namespace Mnk.TBox.Tools.ConsoleScriptRunner.Settings
{
	[Serializable]
	public sealed class Config
	{
		public string SelectedProfile { get; set; }
		public ObservableCollection<Profile> Profiles { get; private set; }

		public Config()
		{
			Profiles = new ObservableCollection<Profile>();
		}
	}
}
