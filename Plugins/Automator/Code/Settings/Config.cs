using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.ScriptEngine;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Plugins.Automator.Code.Settings
{
	[Serializable]
	public sealed class Config : IConfigWithDialogStates
	{
		public string SelectedProfile { get; set; }
		public ObservableCollection<Profile> Profiles { get; set; }
		public IDictionary<string, DialogState> States { get; set; }

		public Config()
		{
			SelectedProfile = "Sample";
			Profiles = new ObservableCollection<Profile>
				{
					new Profile
						{
							Key = "Sample",
							Operations = new ObservableCollection<MultiFileOperation>
								{
									new MultiFileOperation
									{
										Key = "simple script package",
										Pathes = new CheckableDataCollection<CheckableData>{new CheckableData{Key = "Scripts\\params.cs"}} 
									}
								}
						}
				};
			States = new Dictionary<string, DialogState>();
		}

	}
}
