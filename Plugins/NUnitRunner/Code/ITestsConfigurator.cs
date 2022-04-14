using Mnk.ParallelTests.Contracts;
using Mnk.TBox.Plugins.NUnitRunner.Code.Settings;

namespace Mnk.TBox.Plugins.NUnitRunner.Code
{
    public interface ITestsConfigurator
    {
        TestsConfig CreateConfig(string path, TestSuiteConfig suiteConfig);
        void UpdateConfig(TestsConfig config, TestSuiteConfig suiteConfig);
    }
}
