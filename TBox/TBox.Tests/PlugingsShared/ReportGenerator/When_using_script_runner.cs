using System.Collections.Generic;
using System.IO;
using Mnk.TBox.Tests.Common;
using NUnit.Framework;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.Library.ScriptEngine.Core;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Tests.PlugingsShared.ReportGenerator
{
    [TestFixture]
    [Category("Integration")]
    class When_using_report_script_runner : ScriptsFixture
    {
        public static string[] Files
        {
            get { return Directory.GetFiles("../../../bin/"+Shared.CompileMode+"/Data/TeamManager/DataProviders/", "*.cs", SearchOption.TopDirectoryOnly); }
        }

        [Test]
        public void Should_get_packages_for_all_exist_scripts([ValueSource("Files")] string path)
        {
            //Arrange
            var sc = new ScriptCompiler<IReportScript>();

            //Act & Assert
            Assert.IsNotNull(sc.GetPackages(File.ReadAllText(path)));
        }

        [Test]
        public void Should_compile_all_exist_scripts([ValueSource("Files")] string path)
        {
            //Arrange
            var sc = new ScriptCompiler<IReportScript>();

            //Act & Assert
            Assert.IsNotNull(sc.Compile(File.ReadAllText(path), new List<Parameter>()));
        }
    }
}
