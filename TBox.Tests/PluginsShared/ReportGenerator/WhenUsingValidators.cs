﻿using System.IO;
using Mnk.TBox.Tests.Common;
using NUnit.Framework;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.Library.ScriptEngine.Core;

namespace Mnk.TBox.Tests.PluginsShared.ReportGenerator
{
    [TestFixture]
    [Category("Integration")]
    class WhenUsingValidators : ScriptsFixture
    {
        public static string[] Files => Directory.GetFiles("../../../bin/" + Shared.CompileMode + "/Data/TeamManager/Validators/", "*.cs", SearchOption.TopDirectoryOnly);

        [Test]
        public void Should_get_packages_for_all_exist_scripts([ValueSource(nameof(Files))] string path)
        {
            //Arrange
            var sc = new ScriptCompiler<IDayStatusStrategy>();

            //Act & Assert
            Assert.IsNotNull(sc.GetPackages(File.ReadAllText(path)));
        }

        [Test]
        public void Should_compile_all_exist_scripts([ValueSource(nameof(Files))] string path)
        {
            //Arrange
            var sc = new ScriptCompiler<IDayStatusStrategy>();

            //Act & Assert
            Assert.IsNotNull(sc.Compile(File.ReadAllText(path)));
        }
    }
}
