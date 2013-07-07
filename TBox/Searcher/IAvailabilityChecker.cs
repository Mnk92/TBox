using System;

namespace Mnk.Rat
{
    public interface IAvailabilityChecker
    {
        bool CanRebuild { get; }
        bool CanSearch { get; }
        bool CanUnload { get; }
        bool IndexesLoaded { get; }
        event Action OnChanged;
        void BeginOperation();
        void EndMakeIndexes(bool success);
        void EndUnload();
        void EndInitSearch(bool success);
    }
}