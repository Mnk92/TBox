using Mnk.Library.Common.MT;
using Mnk.Rat.Search;

namespace Mnk.Rat
{
    public interface ISearchEngine
    {
        IFileInformer FileInformer { get; }
        IWordsFinder WordsFinder { get; }
        bool LoadSearchInfo(string folderPath, IUpdater updater);
        bool MakeIndex(string folderPath, IUpdater updater);
        void Unload();
    }
}