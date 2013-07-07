using System;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.HtmlPad.Code.Settings
{
    [Serializable]
    public class Profile : Data
    {
        public string Text { get; set; }

        public override object Clone()
        {
            return new Profile
            {
                Key = Key,
                Text = Text
            };
        }
    }
}
