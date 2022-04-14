using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.PasswordsStorage.Code.Settings;

namespace Mnk.TBox.Plugins.PasswordsStorage.Code
{
    interface IPasswordsExporter
    {
        void Import(IConfigManager<Config> cm);
        void Export(IConfigManager<Config> cm);
    }
}
