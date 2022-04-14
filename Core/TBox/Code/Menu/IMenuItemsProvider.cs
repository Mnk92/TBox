using System;
using System.Collections.Generic;
using Mnk.Library.WpfControls.Dialogs.Menu;
using Mnk.Library.WpfControls.Menu;

namespace Mnk.TBox.Core.Application.Code.Menu
{
    public interface IMenuItemsProvider
    {
        event EventHandler OnRefresh;
        event Action<string> OnRefreshItem;

        void Refresh(IList<UMenuItem> items);
        void Create(IList<UMenuItem> items);
        void Refresh(string name, IList<UMenuItem> items);

        UMenuItem Get(string path);
        UMenuItem GetRoot(string path);
        IEnumerable<UMenuItem> GetItems();
        IList<MenuDialogItem> GetDialogItems();
    }
}
