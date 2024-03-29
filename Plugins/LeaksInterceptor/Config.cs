﻿using System.Collections.ObjectModel;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.WpfControls.Dialogs.PerformanceCounters;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Plugins.LeaksInterceptor
{
    [Serializable]
    public class Config : IConfigWithDialogStates
    {
        public int RefreshViewInterval { get; set; }
        public int RefreshDataInterval { get; set; }
        public string LastProcessName { get; set; }
        public CheckableDataCollection<CheckableData> Counters { get; set; }
        public ObservableCollection<SelectedEntity> PerformanceCounters { get; set; }
        public IDictionary<string, DialogState> States { get; set; }

        public Config()
        {
            RefreshViewInterval = 1000;
            RefreshDataInterval = 1000;
            Counters = new CheckableDataCollection<CheckableData>();
            PerformanceCounters = new ObservableCollection<SelectedEntity>();
            States = new Dictionary<string, DialogState>();
        }

    }
}
