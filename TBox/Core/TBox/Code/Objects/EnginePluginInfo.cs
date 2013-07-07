using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using Mnk.Library.Common.UI.Model;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.WpfWinForms;
using Mnk.Library.WpfWinForms.Icons;

namespace Mnk.TBox.Core.Application.Code.Objects
{
	public class EnginePluginInfo : CheckableData
	{
		public PluginGroup PluginGroup { get; set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public Icon Icon { get; private set; }
		public ImageSource ImageSource { get; private set; }

		public IPlugin Plugin { get; set; }

		public EnginePluginInfo(string key, string name, string description, Icon icon, bool isChecked, PluginGroup group)
		{
			PluginGroup = group;
			Key = key;
			Name = name;
			Description = description;
			Icon = icon;
			if(icon!=null)ImageSource = Icon.ToImageSource();
			IsChecked = isChecked;
		}

		public UMenuItem[] Menu 
		{
			get
			{
				return Plugin.Menu;
			}
		}

		public virtual Func<Control> Settings
		{
			get
			{
				var cp = Plugin as IConfigurablePlugin;
				return cp != null ? cp.SettingsGetter : null;
			}
		}

	}
}
