using System;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.XsltTester.Code.Settings
{
    [Serializable]
    public class Profile : Data
    {
        public bool TestManual { get; set; }
        public bool AutoFormatResult { get; set; }
        public string Xslt { get; set; }
        public string Xml { get; set; }

        public Profile()
        {
            TestManual = false;
            AutoFormatResult = true;
        }

        public override object Clone()
        {
            return new Profile
            {
                Key = Key,
                AutoFormatResult = AutoFormatResult,
                TestManual = TestManual,
                Xml = Xml,
                Xslt = Xslt
            };
        }
    }
}
