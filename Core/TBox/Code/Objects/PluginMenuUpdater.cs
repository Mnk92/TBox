using System;

namespace Mnk.TBox.Core.Application.Code.Objects
{
	class PluginMenuUpdater
	{
		private readonly Action<string> action;
		private string name;
		public PluginMenuUpdater(Action<string> action)
		{
			this.action = action;
		}
		public void Init(string pluginName)
		{
			name = pluginName;
		}
		public void Do()
		{
			action(name);
		}
	}
}
