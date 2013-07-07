using System;
using Mnk.Library.Common.Network;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.Requestor.Code.Settings
{
	[Serializable]
	public sealed class Header : Data, IHeader
	{
		public string Value { get; set; }
		public override object Clone()
		{
			return new Header { Key = Key, Value = Value };
		}
	}
}
