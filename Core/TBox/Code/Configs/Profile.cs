using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Core.Application.Code.Configs
{
    public class Profile : Data
    {
        public CheckableDataCollection<CheckableData> Selected { get; set; }

        public Profile()
        {
            Selected = new CheckableDataCollection<CheckableData>();
        }

        public override object Clone()
        {
            return new Profile
                {
                    Key = Key,
                    Selected = Selected.Clone()
                };
        }
    }
}
