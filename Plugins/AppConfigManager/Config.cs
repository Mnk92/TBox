using System;
using System.Collections.ObjectModel;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.TBox.Plugins.AppConfigManager.Code;

namespace Mnk.TBox.Plugins.AppConfigManager
{
	[Serializable]
	public class Config
	{
		public string SelectedProfile { get; set; }
		public string ItemSourceTemplate { get; set; }
		public string ItemResultTemplate { get; set; }
		public string DefaultValue { get; set; }
		public string LastBuildValue { get; set; }
		public ObservableCollection<Profile> Profiles { get; set; }

		public Config()
		{
			ItemSourceTemplate = "showfeature{item}";
			ItemResultTemplate = "Show.Feature.{item}";
			Profiles = new ObservableCollection<Profile>
				{
					new Profile
						{
							Key = "Sample",
							Options = new CheckableDataCollection<Option>
								{
									new Option{Key = "SampleFeature", Value = "True"}
								},
							Files = new CheckableDataCollection<CheckableData>
								{
									new CheckableData{Key = "c:\\Web.config"}
								} 
							
						}
				};
			SelectedProfile = "Sample";
		}
	}
}
