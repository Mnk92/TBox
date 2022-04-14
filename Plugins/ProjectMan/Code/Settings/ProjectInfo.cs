using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.ProjectMan.Code.Settings
{
	public class ProjectInfo : CheckableData
	{
		public string MsBuildParams { get; set; }

		public ProjectInfo()
		{
			MsBuildParams = string.Empty;
		}

		public override object Clone()
		{
			return new ProjectInfo
				{
					IsChecked = IsChecked,
					Key = Key,
					MsBuildParams = MsBuildParams
				};
		}
	}
}
