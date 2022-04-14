using Mnk.Library.WpfControls.Menu;

namespace Mnk.TBox.Core.Application.Code.Menu
{
    interface IMenuRunHandler
    {
        void Handle(UMenuItem item, string[] path);
    }
}
