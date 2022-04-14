using System.Threading.Tasks;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Translate
{
    interface ITranslator
    {
        Task<string> Translate(string text, string locFrom, string locTo);
    }
}