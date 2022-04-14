using Mnk.TBox.Plugins.NUnitRunner.Components;

namespace Mnk.TBox.Plugins.NUnitRunner.Code
{
    static class TestsStateSingleton
    {
        class TestState
        {
            public bool State { get; set; }
            public Result Result { get; set; }
        }
        private static readonly IDictionary<string, TestState> Results = new Dictionary<string, TestState>();

        public static void Clear()
        {
            lock (Results)
            {
                Results.Clear();
            }
        }

        public static bool IsRunning(Result result)
        {
            lock (Results)
            {
                return Results.TryGetValue(result.Key, out var value) && value.State;
            }
        }

        public static void SetItems(IEnumerable<Result> results)
        {
            lock (Results)
            {
                Results.Clear();
                foreach (var result in results.GroupBy(x => x.Key))
                {
                    Results.Add(result.Key,
                        new TestState
                        {
                            State = true,
                            Result = result.First()
                        });
                }
            }
        }

        public static void SetFinished(IList<Result> exists)
        {
            lock (Results)
            {
                foreach (var x in exists)
                {
                    var testState = Results[x.Key];
                    testState.State = false;
                    testState.Result.Refresh();
                }
            }
        }
    }
}
