using System.Linq;
using Mnk.Library.ParallelNUnit;
using Mnk.TBox.Plugins.NUnitRunner.Code.Settings;

namespace Mnk.TBox.Plugins.NUnitRunner.Code
{
    class TestsConfigurator : ITestsConfigurator
    {
        private readonly string nunitAgentPath;
        private readonly string runAsx86Path;

        public TestsConfigurator(string nunitAgentPath, string runAsx86Path)
        {
            this.nunitAgentPath = nunitAgentPath;
            this.runAsx86Path = runAsx86Path;
        }

        public TestsConfig CreateConfig(string path, TestSuiteConfig suiteConfig)
        {
            var config = new TestsConfig
            {
                NunitAgentPath = nunitAgentPath,
                RunAsx86Path = runAsx86Path,
                TestDllPath = path,
            };
            UpdateConfig(config, suiteConfig);
            return config;
        }

        public void UpdateConfig(TestsConfig config, TestSuiteConfig suiteConfig)
        {
            config.RunAsx86 = suiteConfig.RunAsx86;
            config.RunAsAdmin = suiteConfig.RunAsAdmin;
            config.DirToCloneTests = suiteConfig.DirToCloneTests;
            config.CommandBeforeTestsRun = suiteConfig.CommandBeforeTestsRun;
            config.RuntimeFramework = suiteConfig.RuntimeFramework;
            config.ProcessCount = suiteConfig.ProcessCount;
            config.OptimizeOrder = suiteConfig.UsePrefetch;
            config.IncludeCategories = suiteConfig.UseCategories ? (bool?)suiteConfig.IncludeCategories : null;
            config.CopyToSeparateFolders = suiteConfig.CopyToSeparateFolders;
            config.CopyMasks = suiteConfig.CopyMasks.CheckedItems.Select(x => x.Key).ToArray();
            config.NeedSynchronizationForTests = suiteConfig.NeedSynchronizationForTests && suiteConfig.ProcessCount > 1;
            config.StartDelay = suiteConfig.StartDelay;
            config.Timeout = suiteConfig.Timeout*1000;
            config.Mode = suiteConfig.Mode;
            config.NeedOutput = true;
            config.SkipChildrenOnCalculateTests = true;
        }
    }
}
