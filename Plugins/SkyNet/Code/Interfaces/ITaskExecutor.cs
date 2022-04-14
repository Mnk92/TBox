using Mnk.Library.ScriptEngine;

namespace Mnk.TBox.Plugins.SkyNet.Code.Interfaces
{
    public interface ITaskExecutor
    {
        Task<TaskInfo> Execute(SingleFileOperation operation);
    }
}