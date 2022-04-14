using System.Collections.Generic;

namespace Mnk.TBox.Core.PluginsShared.LoadTesting
{
	public interface IProfile
	{
		string Key { get; }
		IEnumerable<IOperation> GetOperations();
	}
}
