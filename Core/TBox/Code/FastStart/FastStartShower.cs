using System;
using Mnk.TBox.Core.Application.Code.FastStart.Settings;

namespace Mnk.TBox.Core.Application.Code.FastStart
{
	class FastStartShower
	{
		private readonly Action showSettings;
		private readonly Action showFastStart;
		private FastStartConfig config;

		public FastStartShower(Action showSettings, Action showFastStart)
		{
			this.showSettings = showSettings;
			this.showFastStart = showFastStart;
		}

		public void Load(FastStartConfig cfg)
		{
			config = cfg;
		}

		public void Show()
		{
			if (config.IsFastStart) showFastStart();
			else showSettings();
		}

		public void ShowSettings()
		{
			config.IsFastStart = false;
			showSettings();
		}

		public void ShowFastStart()
		{
			config.IsFastStart = true;
			showFastStart();
		}
	}
}
