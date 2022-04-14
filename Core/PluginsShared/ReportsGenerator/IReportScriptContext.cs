using System;
using System.Collections.Generic;

namespace Mnk.TBox.Core.PluginsShared.ReportsGenerator
{
    public interface IReportScriptContext
    {
        string Name { get; }
        DateTime DateFrom { get; }
        DateTime DateTo { get; }
        IList<string> Persons { get; }
        void AddResult(LoggedInfo info);
        void Configure(bool hasTime);
    }
}
