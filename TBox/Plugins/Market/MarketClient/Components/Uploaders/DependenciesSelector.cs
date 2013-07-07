using System;
using System.Linq;
using System.Windows;
using Mnk.TBox.Plugins.Market.Client.ServiceReference;
using Mnk.Library.WpfControls.Code.Dialogs;

namespace Mnk.TBox.Plugins.Market.Client.Components.Uploaders
{
	public sealed class DependenciesSelector : BaseDialog
	{
		public const string Divider = ">";
		private readonly DependencyChooserDialog dialog = new DependencyChooserDialog();

        public DependenciesSelector(string caption, Templates templates, Func<string, bool> validator, Func<Window> ownerGetter) :
			base(caption, templates, validator, ownerGetter)
		{
			dialog.Chooser.OnAction += OnAction;
		}

		private bool userPressActionButton;
		private void OnAction(object sender, EventArgs e)
		{
			userPressActionButton = true;
			dialog.Hide();
		}

		public override bool Add(out string[] newNames)
		{
			userPressActionButton = false;
			dialog.ShowDialog(Validator);
			if ( userPressActionButton )
			{
				newNames = dialog.Chooser.Items.Select(FormatName).ToArray();
				return true;
			}
			newNames = new string[0];
			return false;
		}

		public override bool Edit(string name, out string newName)
		{
			newName = string.Empty;
			return false;
		}

		public static string FormatName(Plugin p)
		{
			return p.Author + Divider + p.Name;
		}

	}
}
