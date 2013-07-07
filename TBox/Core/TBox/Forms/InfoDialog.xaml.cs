using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Navigation;
using Mnk.TBox.Locales.Localization.TBox;

namespace Mnk.TBox.Core.Application.Forms
{
    /// <summary>
    /// Interaction logic for InfoDialog.xaml
    /// </summary>
    public partial class InfoDialog 
    {
        public InfoDialog()
        {
            InitializeComponent();
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            Version.Content = string.Format(TBoxLang.VersionTemplate, ver.Major, ver.Minor, ver.MajorRevision, ver.MinorRevision);

            const string changeLogFilePath = "changelog.txt";
            if (File.Exists(changeLogFilePath))
            {
                tbChangeLog.Text = File.ReadAllText(changeLogFilePath);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            using (Process.Start(((Hyperlink)sender).NavigateUri.ToString())) { }
        }

    }
}
