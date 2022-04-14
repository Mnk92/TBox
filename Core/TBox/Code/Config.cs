using System;
using System.Collections.Generic;
using Mnk.TBox.Core.Application.Code.AutoUpdate;
using Mnk.TBox.Core.Application.Code.Configs;
using Mnk.TBox.Core.Application.Code.FastStart.Settings;
using Mnk.TBox.Core.Application.Code.HotKeys.Settings;
using Mnk.TBox.Core.Application.Code.Shelduler.Settings;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Core.Application.Code
{
	[Serializable]
	public sealed class Config
	{
		public bool HideOnSave { get; set; }
		public bool HideOnCancel { get; set; }
		public bool StartHidden { get; set; }
		public bool UseMenuWithIcons { get; set; }
		public bool ShowSettingsByTraySingleClick { get; set; }
		public string Theme { get; set; }
		public string Language { get; set; }

		public Update Update { get; set; }
		public HotkeyTasks HotKeys { get; set; }
		public SchedulerTasks SchedulerTasks { get; set; }
		public List<string> DisabledItems { get; set; }
		public DialogState DialogState { get; set; }
		public FastStartConfig FastStartConfig { get; set; }
		public bool UpdateFromSharedlFolder { get; set; }
		public string LastKnownVersion { get; set; }
		public bool EnableGPUAccelerationForUi { get; set; }
		public string FeedBackMessage { get; set; }
        public Configuration Configuration { get; set; }

	    public Config()
		{
			HideOnSave = false;
			HideOnCancel = false;
			StartHidden = false;
			Update = new Update();
			HotKeys = new HotkeyTasks();
			SchedulerTasks  = new SchedulerTasks();
			DisabledItems = new List<string>();
			FastStartConfig = new FastStartConfig();
			Theme = "Default.xaml";
			Language = "en";
			UseMenuWithIcons = true;
			ShowSettingsByTraySingleClick = true;
			UpdateFromSharedlFolder = false;
			EnableGPUAccelerationForUi = false;
			FeedBackMessage = string.Empty;
            Configuration = new Configuration();
		}
	}
}
