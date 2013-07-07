using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Mnk.Library.Common;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Plugins.XsltTester.Code.Settings;

namespace Mnk.TBox.Plugins.XsltTester
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        public Dialog()
        {
            InitializeComponent();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (TestManual.IsChecked == true) return;
            DoTest();
        }

        private void ButtonTestClick(object sender, RoutedEventArgs e)
        {
            DoTest();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnShow()
        {
            base.OnShow();
            Xslt.SetFocus();
        }

        private void DoTest()
        {
            sbError.Visibility = sbErrorCaption.Visibility = Visibility.Hidden;
            Results.Clear();
            stopwatch.Restart();
            ExceptionsHelper.HandleException(
                () => Execute(Xslt.Text, Xml.Text),
                ex =>
                {
                    sbError.Content = ex.Message;
                    sbError.Visibility = sbErrorCaption.Visibility = Visibility.Visible;
                }
                );
        }

        private void Execute(string xslt, string xml)
        {
            var transform = new XslCompiledTransform();
            using (var xmlStringReader = new StringReader(xml))
            using (var xmlReader = XmlReader.Create(xmlStringReader))
            using (var xsltStringReader = new StringReader(xslt))
            using (var xsltReader = XmlReader.Create(xsltStringReader))
            using (var xmlStringWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(xmlStringWriter, new XmlWriterSettings{ConformanceLevel = ConformanceLevel.Auto}))
            {
                transform.Load(xsltReader);
                transform.Transform(xmlReader, new XsltArgumentList(), xmlWriter);
                var str = xmlStringWriter.ToString();
                if (AutoFormatResult.IsChecked == true)
                {
                    try
                    {
                        str = XElement.Parse(str).ToString();
                    }
                    catch { }
                }
                Results.Text = str;
                sbTime.Content = stopwatch.ElapsedMilliseconds;
            }
        }
    }
}
