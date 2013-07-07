using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Plugins.ServicesCommander.Code
{
    public class Profile : Data
    {
        public CheckableDataCollection<ServiceInfo> Services { get; set; }

        public Profile()
        {
            Services = new CheckableDataCollection<ServiceInfo>();
        }

        public override object Clone()
        {
            return new Profile { Key = Key, Services = Services.Clone() };
        }
    }
}
