using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Tools.ConsoleScriptRunner
{
    class CopyPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
           return path;
        }
    }
}
