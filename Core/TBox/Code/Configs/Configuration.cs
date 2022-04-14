using System.Collections.ObjectModel;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Core.Application.Code.Configs
{
    public class Configuration
    {
        public bool PortableMode { get; set; }
        public CheckableDataCollection<CheckableData<string>> Aliases { get; set; }

        public Configuration()
        {
            PortableMode = false;
            Aliases = new CheckableDataCollection<CheckableData<string>>();
        }
    }
}
