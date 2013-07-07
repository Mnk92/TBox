using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mnk.Rat;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.Searcher.Code.Settings;

namespace Mnk.TBox.Plugins.Searcher.Code.Rat
{
    class IndexContextBuilder : IIndexContextBuilder
    {
        private readonly IConfigManager<Config> cm;
        public IndexContextBuilder(IConfigManager<Config> cm)
        {
            this.cm = cm;
            Rebuild();
        }

        public void Rebuild()
        {
            var index = cm.Config.Index;
            Context = new IndexContext
            {
                DecodeStrings = index.DecodeStrings,
                SkipComments = index.SkipComments,
                TargetDirectories = index.FileNames.CheckedItems.Select(x=>x.Key).ToList(),
                TargetFileTypes = index.FileTypes.CheckedItems.Select(x=>x.Key).ToList(),
                SearchableCharacters = new HashSet<char>(index.SearchableCharacters),
            };
            Filter = index.FileMasksToExclude
                .Select(x => new Regex(BuildReqexp(x.Key), RegexOptions.Compiled | RegexOptions.IgnoreCase))
                .ToArray();
        }

        public IndexContext Context { get; private set; }
        public Regex[] Filter { get; private set; }

        private static string BuildReqexp(string x)
        {
            return x
                .Replace(".", "[.]")
                .Replace("*", ".*")
                .Replace("?", ".")
                .Replace("\\", "\\\\")
                + "$";
        }

    }
}