using System;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.TBox.Locales.Localization.Plugins.HtmlPad;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Plugins.HtmlPad
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog
    {
        private static readonly ILog Log = LogManager.GetLogger<Dialog>();
        public Dialog()
        {
            InitializeComponent();
            Editor.Format = "html";
            Editor.TextChanged += EditorOnTextChanged;
        }

        private void EditorOnTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Editor.Text))
            {
                ExceptionsHelper.HandleException(
                    () => Html.NavigateToString(Editor.Text),
                    () => "Can't set html body",
                    Log
                    );
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
            Editor.Focus();
        }
    }
}
