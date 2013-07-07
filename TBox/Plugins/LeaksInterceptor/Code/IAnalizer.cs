using System.Collections.Generic;
using Mnk.Library.WpfControls.Components.Drawings.Graphics;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code
{
	interface IAnalizer
	{
		bool Analize();
		string CopyResults();
		IGraphic GetGraphic(string name);
		IEnumerable<string> GetNames();
	}
}
