using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.ServicesCommander.Code
{
    public class ServiceInfo : CheckableData
    {
        public override object Clone()
        {
            return new ServiceInfo
            {
                IsChecked = IsChecked,
                Key = Key
            };
        }
    }
}
