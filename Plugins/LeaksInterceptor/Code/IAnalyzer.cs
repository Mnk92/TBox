using Mnk.Library.WpfControls.Components.Drawings.Graphics;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code
{
    interface IAnalyzer
    {
        bool Analyze();
        string CopyResults();
        IGraphic GetGraphic(string name);
        IEnumerable<string> GetNames();
    }
}
