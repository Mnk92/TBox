using Mnk.Library.ScriptEngine;

namespace Mnk.TBox.Plugins.SkyNet.Code.Interfaces
{
    public interface ITaskExecutor
    {
        TaskInfo Execute(SingleFileOperation operation);
    }
}