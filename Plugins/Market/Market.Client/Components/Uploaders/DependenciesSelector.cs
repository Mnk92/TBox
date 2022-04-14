using System.Windows;
using Mnk.Library.WpfControls.Code.Dialogs;
using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Client.Components.Uploaders
{
    public sealed class DependenciesSelector : BaseDialog
    {
        public static readonly char Divider = '>';
        private readonly DependencyChooserDialog dialog = new();

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
            if (userPressActionButton)
            {
                newNames = dialog.Chooser.Items.Select(FormatName).ToArray();
                return true;
            }
            newNames = Array.Empty<string>();
            return false;
        }

        public override bool Edit(string name, out string newName)
        {
            newName = string.Empty;
            return false;
        }

        public static string FormatName(Plugin p)
        {
            return $"{p.Author}{Divider}{p.Name}";
        }

    }
}
