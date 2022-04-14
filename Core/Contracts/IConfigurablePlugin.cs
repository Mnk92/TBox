using System;
using System.Windows.Controls;
using Mnk.Library.Common.Plugins;

namespace Mnk.TBox.Core.Contracts
{
	public interface IConfigurablePlugin : IPlugin, IConfigurable
	{
		void Load();
		void Save(bool autoSaveOnExit);
		void OnRebuildMenu();
		Func<Control> SettingsGetter { get; }
	}
}
