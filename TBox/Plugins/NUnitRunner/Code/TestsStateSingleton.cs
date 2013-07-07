using System.Collections.Generic;
using System.Linq;
using Mnk.Library.ParallelNUnit.Core;

namespace Mnk.TBox.Plugins.NUnitRunner.Code
{
    static class TestsStateSingleton
    {
        class TestState
        {
            public bool State { get; set; }
            public Result Result { get; set; }
        }
        private readonly static IDictionary<string, TestState> Results = new Dictionary<string, TestState>();

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
                TestState value;
                return Results.TryGetValue(result.Key, out value) && value.State;
            }
        }

        public static void SetItems(IEnumerable<Result> results)
        {
            lock (Results)
            {
                Results.Clear();
                foreach (var result in results.GroupBy(x=>x.Key))
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
