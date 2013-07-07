using System;
using System.IO;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Core.Application.Code.Configs
{
    class PathResolver : IPathResolver
    {
        private readonly IConfigManager<Config> cm;
        public PathResolver(IConfigManager<Config> cm)
        {
            this.cm = cm;
        }

        public string Resolve(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;
            foreach (var aliase in cm.Config.Configuration.Aliases.CheckedItems)
            {
                var max = 10;
                while (--max>0)
                {
                    var id = path.IndexOf(aliase.Key, StringComparison.OrdinalIgnoreCase);
                    if (id < 0)break;
                    path = path
                        .Remove(id, aliase.Key.Length)
                        .Insert(id, aliase.Value);
                }
            }
            return Path.GetFullPath(path);
        }
    }
}
