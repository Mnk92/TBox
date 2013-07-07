using System;

namespace Mnk.Rat
{
    sealed class AvailabilityChecker : IAvailabilityChecker
    {
        public event Action OnChanged;
        public bool CanSearch { get; private set; }
        public bool CanUnload { get; private set; }
        public bool CanRebuild { get; private set; }
        public bool IndexesLoaded { get; private set; }

        public AvailabilityChecker()
        {
            CanSearch = true;
            CanUnload = false;
            CanRebuild = true;
            IndexesLoaded = false;
        }

        public void BeginOperation()
        {
            DisableControls();
        }

        private void DisableControls()
        {
            CanSearch = false;
            CanRebuild = false;
            CanUnload = false;
            ExecuteEvent();
        }

        public void EndMakeIndexes(bool success)
        {
            CanSearch = CanUnload = success;
            CanRebuild = true;
            IndexesLoaded = success;
            ExecuteEvent();
        }

        public void EndUnload()
        {
            CanSearch = true;
            CanUnload = false;
            CanRebuild = true;
            IndexesLoaded = false;
            ExecuteEvent();
        }

        public void EndInitSearch(bool success)
        {
            CanSearch = CanUnload = success;
            CanRebuild = true;
            IndexesLoaded = success;
            ExecuteEvent();
        }

        private void ExecuteEvent()
        {
            var e = OnChanged;
            if (e != null) e();
        }

    }
}
