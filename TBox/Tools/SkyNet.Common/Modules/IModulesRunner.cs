using System;

namespace Mnk.TBox.Tools.SkyNet.Common.Modules
{
    public interface IModulesRunner : IDisposable
    {
        void Start();
        void Stop();
    }
}
