using System;
using System.Collections.Generic;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.TBox.Core.PluginsShared.LoadTesting;

namespace Mnk.TBox.Plugins.Requester.Code.Settings
{
	[Serializable]
	public sealed class Profile : Data, IProfile
	{
		public CheckableDataCollection<Op> Ops { get; set; }

		public Profile()
		{
			Ops = new CheckableDataCollection<Op>();
		}

		public override object Clone()
		{
			return new Profile
			{
				Ops = Ops.Clone(),
			};
		}

		public IEnumerable<IOperation> GetOperations()
		{
			return Ops.CheckedItems;
		}
	}
}
