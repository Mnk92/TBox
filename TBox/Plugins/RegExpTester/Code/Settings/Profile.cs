using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.RegExpTester.Code.Settings
{
    [Serializable]
    public class Profile : Data
    {
        public bool TestManual { get; set; }
        public string RegExp { get; set; }
        public string Text { get; set; }
        public int Options { get; set; }

        public Profile()
        {
            Options = (int)RegexOptions.Compiled;
        }

        public override object Clone()
        {
            return new Profile
            {
                Key = Key,
                Options = Options,
                RegExp = RegExp,
                TestManual = TestManual,
                Text = Text
            };
        }
    }
}
