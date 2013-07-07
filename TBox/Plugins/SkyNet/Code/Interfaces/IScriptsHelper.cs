using System.Collections.Generic;
using Mnk.Library.ScriptEngine;

namespace Mnk.TBox.Plugins.SkyNet.Code.Interfaces
{
    public interface IScriptsHelper
    {
        IList<string> GetPaths();
        void ShowParameters(SingleFileOperation operation);
    }
}
