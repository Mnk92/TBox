using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.XsltTester.Code.Settings
{
    [Serializable]
    public class Config : IConfigWithDialogStates
    {
        public IDictionary<string, DialogState> States { get; set; }
        public ObservableCollection<Profile> Profiles { get; set; } 
        public string SelectedProfile { get; set; }

        public Config()
        {
            Profiles = new ObservableCollection<Profile>
            {
                new Profile
                {
                    Key = "Default",
                    Xml = 
@"<class>
<item1>Ivan</item1>
<item2>Petrovich</item2>
</class>",
                    Xslt = 
@"<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
	<xsl:template match='item2'>
		<u><xsl:value-of select='.'/></u>
	</xsl:template>
	<xsl:template match='item1'>
		<p><xsl:value-of select='.'/></p>
	</xsl:template>
</xsl:stylesheet>"
                }
            };
            SelectedProfile = "Default";
            States = new Dictionary<string, DialogState>();
        }
    }
}
