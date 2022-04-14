using System.IO;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.ParallelTests.Contracts;

namespace Mnk.TBox.Plugins.NUnitRunner.Code.Settings
{
    [Serializable]
    public class TestSuiteConfig : CheckableData
    {
        public bool MultiThreaded { get; set; }
        public bool CopyToSeparateFolders { get; set; }
        public int ProcessCount { get; set; }
        public int AssembliesCount { get; set; }
        public CheckableDataCollection<CheckableData> CopyMasks { get; set; }
        public CheckableDataCollection<CheckableData> FilePaths { get; set; }
        public bool RunAsx86 { get; set; }
        public bool RunAsAdmin { get; set; }
        public bool NeedSynchronizationForTests { get; set; }
        public string DirToCloneTests { get; set; }
        public string CommandBeforeTestsRun { get; set; }
        public bool UseCategories { get; set; }
        public bool IncludeCategories { get; set; }
        public bool UsePrefetch { get; set; }
        public int StartDelay { get; set; }
        public int Timeout { get; set; }
        public string RuntimeFramework { get; set; }
        public string Mode { get; set; }

        public TestSuiteConfig()
        {
            CopyToSeparateFolders = false;
            ProcessCount = 1;
            AssembliesCount = 1;
            FilePaths = new CheckableDataCollection<CheckableData>();
            CopyMasks = new CheckableDataCollection<CheckableData>
            {
                new CheckableData { Key = "*.dll" },
                new CheckableData { Key = "*.config"}
            };
            RunAsx86 = true;
            RunAsAdmin = false;
            NeedSynchronizationForTests = false;
            DirToCloneTests = Path.GetTempPath();
            CommandBeforeTestsRun = string.Empty;
            UseCategories = false;
            IncludeCategories = true;
            StartDelay = 0;
            Timeout = 120;
            UsePrefetch = false;
            RuntimeFramework = "";
            Mode = TestsRunnerMode.Process;
        }

        public override object Clone()
        {
            return new TestSuiteConfig
            {
                IsChecked = IsChecked,
                Key = Key,
                CopyToSeparateFolders = CopyToSeparateFolders,
                ProcessCount = ProcessCount,
                AssembliesCount = AssembliesCount,
                CopyMasks = CopyMasks.Clone(),
                FilePaths = FilePaths.Clone(),
                RunAsx86 = RunAsx86,
                RunAsAdmin = RunAsAdmin,
                NeedSynchronizationForTests = NeedSynchronizationForTests,
                MultiThreaded = MultiThreaded,
                DirToCloneTests = DirToCloneTests,
                CommandBeforeTestsRun = CommandBeforeTestsRun,
                UseCategories = UseCategories,
                IncludeCategories = IncludeCategories,
                StartDelay = StartDelay,
                Timeout = Timeout,
                UsePrefetch = UsePrefetch,
                RuntimeFramework = RuntimeFramework
            };
        }
    }
}
