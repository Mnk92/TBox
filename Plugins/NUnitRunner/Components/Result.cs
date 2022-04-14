using Mnk.Library.Common.Models;
using Mnk.ParallelTests.Contracts;

namespace Mnk.TBox.Plugins.NUnitRunner.Components
{
    public class Result : IHasChildren, IRefreshable
    {
        private ResultMessage message;

        public ResultMessage InternalResult
        {
            get => message;
            set
            {
                message = value;
                Children = message.Children.Select(x => new Result(x)).ToArray();
            }
        }
        public int Id => InternalResult.Id;
        public string Description => InternalResult.Description;
        public string Key => InternalResult.Key;
        public string FullName => InternalResult.FullName;
        public string Type => InternalResult.Type;
        public string Message => InternalResult.Message;
        public string StackTrace => InternalResult.StackTrace;
        public ResultMessage.Types.TestResultState State => InternalResult.State;
        public ResultMessage.Types.TestFailedOn FailureSite => InternalResult.FailureSite;
        public string[] Categories => InternalResult.Categories.ToArray();
        public double Duration => InternalResult.Duration;
        public int AssertCount => InternalResult.AssertCount;
        public string Output => InternalResult.Output;

        public IList<IHasChildren> Children { get; set; }

        public event Action OnRefresh;

        public Result(ResultMessage result = null)
        {
            if (result == null)
            {
                message = new ResultMessage
                {
                    State = ResultMessage.Types.TestResultState.NotRunnable,
                    FailureSite = ResultMessage.Types.TestFailedOn.Test
                };
                Children = Array.Empty<IHasChildren>();
            }
            else
            {
                message = result;
                Children = message.Children.Select(x => new Result(x)).ToArray();
            }
        }

        public virtual void Refresh()
        {
            OnRefresh?.Invoke();
        }

        public bool IsTest => InternalResult.IsTest();

        public bool Executed => InternalResult.Executed();

        public bool IsSuccess => InternalResult.IsSuccess();

        public bool IsFailure => InternalResult.IsFailure();

        public bool IsError => InternalResult.IsError();

        public bool HasResults => InternalResult.HasResults();
    }
}
