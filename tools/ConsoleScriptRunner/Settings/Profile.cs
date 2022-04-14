using System;
using System.Collections.ObjectModel;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.ScriptEngine;

namespace Mnk.TBox.Tools.ConsoleScriptRunner.Settings
{
	[Serializable]
	public sealed class Profile : Data
	{
		public ObservableCollection<MultiFileOperation> Operations { get; private set; }

		public Profile()
		{
			Operations = new ObservableCollection<MultiFileOperation>();
		}

		public override object Clone()
		{
			return new Profile
			{
				Operations = Operations.Clone(),
			};
		}
	}
}
