using System.Collections.Generic;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Core.Contracts
{
	public interface IConfigWithDialogStates
	{
		IDictionary<string, DialogState> States { get; set; }
	}
}
