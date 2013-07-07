using System;
using System.Drawing;
using System.Windows.Controls;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Core.Application.Code.Objects
{
	class EngineSettingsInfo : EnginePluginInfo
	{
		private readonly Func<Control> settings;

		public EngineSettingsInfo(string name, string description, Icon icon, Func<Control> settings):base(name,name,description,icon,true,PluginGroup.Other)
		{
			this.settings = settings;
		}

		public override Func<Control> Settings
		{
			get
			{
				return settings;
			}
		}
	}
}
