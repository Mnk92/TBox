using System.Collections.Generic;

namespace Mnk.TBox.Core.PluginsShared.Templates
{
	interface IKnownValues
	{
		IDictionary<string, string> Collection { get; }
		void Prepare(IStringFiller filler);
	}
}
