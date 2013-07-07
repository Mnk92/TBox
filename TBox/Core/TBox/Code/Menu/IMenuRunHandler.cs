using Mnk.Library.WpfWinForms;

namespace Mnk.TBox.Core.Application.Code.Menu
{
	interface IMenuRunHandler
	{
		void Handle(UMenuItem item, string[]path);
	}
}
