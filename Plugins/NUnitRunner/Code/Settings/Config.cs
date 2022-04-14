using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.NUnitRunner.Code.Settings
{
    [Serializable]
    public class Config : IConfigWithDialogStates
    {
        public CheckableDataCollection<TestSuiteConfig> TestSuites { get; set; }
        public IDictionary<string, DialogState> States { get; set; }

        public Config()
        {
            TestSuites = new CheckableDataCollection<TestSuiteConfig>
            {
                new TestSuiteConfig
                {
                    Key = "Default",
                }
            };
            States = new Dictionary<string, DialogState>();
        }
    }
}
