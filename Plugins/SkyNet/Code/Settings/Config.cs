using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.ScriptEngine;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Plugins.SkyNet.Code.Settings
{
    public class Config : IConfigWithDialogStates
    {
        public IDictionary<string, DialogState> States { get; set; }
        public CheckableDataCollection<SingleFileOperation> Operations { get; set; }
        public string AgentEndpoint { get; set; }
        public string ServerEndpoint { get; set; }

        public Config()
        {
            States = new Dictionary<string, DialogState>();
            Operations = new CheckableDataCollection<SingleFileOperation>();
            AgentEndpoint = "http://localhost:6002";
            ServerEndpoint = "http://localhost:6003";
        }
    }
}
