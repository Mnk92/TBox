using System;
using System.Collections.Generic;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Plugins.SourcesUniter.Code
{
	[Serializable]
	public class Config : IConfigWithDialogStates
	{
		public string Path { get; set; }
		public string Extensions { get; set; }
		public string Editor { get; set; }
		public bool RemoveEmptyLines { get; set; }
		public IDictionary<string, DialogState> States { get; set; }

		public Config()
		{
			Path = Environment.CurrentDirectory;
			Extensions = "*.cs;*.xaml";
			Editor = "notepad.exe";
			RemoveEmptyLines = true;
			States = new Dictionary<string, DialogState>();
		}

	}
}
