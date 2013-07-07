using System.Windows;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code.FastStart.Settings;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Core.Application.Forms
{
	/// <summary>
	/// Interaction logic for OperationDialog.xaml
	/// </summary>
	public partial class UserActionsDialog 
	{
		public UserActionsDialog()
		{
			InitializeComponent();
		}

		public void ShowDialog(MenuItemsSequence operation, IMenuItemsProvider menuItemsProvider)
		{
			Owner = System.Windows.Application.Current.MainWindow;
			DataContext = operation;
			Paths.ConfigureInputMenuItem(TBoxLang.SelectMenuActions, 
				operation.MenuItems, 
				menuItemsProvider.GetDialogItems());
			SafeShowDialog();
		}
	}
}
