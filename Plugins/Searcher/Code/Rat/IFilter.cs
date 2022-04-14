using ZetaLongPaths;
using ZetaLongPaths.Native;

namespace Mnk.TBox.Plugins.Searcher.Code.Rat
{
    interface IFilter
    {
        bool CanInclude(ZlpFileInfo info);
        bool CheckAttribute(FileAttributes a);
    }
}