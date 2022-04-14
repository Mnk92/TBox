using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Agent.Code
{
    class SkyContext : ISkyContext
    {
        private AgentTask currentTask;
        public void Dispose()
        {
        }

        public void Update(float value)
        {
            currentTask.Progress = (int)(value * 100);
        }

        public void Update(string caption, float value)
        {
            Update(value);
        }

        public bool UserPressClose { get; private set; }
        public void Update(Func<int, string> caption, int current, int total)
        {
            Update(current / (float)total);
        }

        public void Reset(AgentTask task)
        {
            currentTask = task;
            UserPressClose = false;
        }

        public void Cancel()
        {
            UserPressClose = true;
        }
    }
}
