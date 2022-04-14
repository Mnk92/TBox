using System;
using System.Collections.ObjectModel;

namespace Mnk.Rat.Settings
{
    [Serializable]
    public sealed class SearchConfig
    {
        public int FileCount { get; set; }
        public SearchMode SearchMode { get; set; }
        public bool MatchCase { get; set; }
        public bool FullTextSearch { get; set; }
        public CompareType CompareType { get; set; }
        public string SearchText { get; set; }
        public ObservableCollection<string> LastSearchValues { get; set; }

        public SearchConfig()
        {
            FileCount = 256;
            SearchMode = SearchMode.FileData;
            CompareType = CompareType.Contain;
            SearchText = string.Empty;
            LastSearchValues = new ObservableCollection<string>();
            FullTextSearch = false;
            MatchCase = false;
        }
    }
}
