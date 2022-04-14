using System;
using System.Windows;
using System.Windows.Media;
using Mnk.Library.Common.Network;
using Mnk.TBox.Locales.Localization.Plugins.Requestor;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Plugins.Requester.Code.Settings;

namespace Mnk.TBox.Plugins.Requester.Code
{
	public class BaseExecutor
	{
		private readonly Request r = new Request();

		public void Execute(Window owner, Op operation, Action<ResponseInfo> onEnd, ImageSource icon)
		{
			DialogsCache.ShowProgress(
				u => Work(
					(Op)operation.Clone(),
					onEnd
                    ), RequestorLang.MakeRequestTo + operation.Key, owner, icon:icon);
		}

		private void Work(Op op, Action<ResponseInfo> onEnd)
		{
			var response = r.GetResult(op.Request.Url, op.Request.Method, op.Request.Body, op.Request.Headers);
			onEnd(response);
		}
	}
}
