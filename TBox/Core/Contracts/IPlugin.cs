using System.Drawing;
using System.Windows.Media;
using Mnk.Library.WpfWinForms;

namespace Mnk.TBox.Core.Contracts 
{
	public interface IPlugin
	{
		void Init(IPluginContext context);
		UMenuItem[] Menu { get; }
		Icon Icon { get; set; }
		PluginGroup PluginGroup { get; set; }
	}
}
