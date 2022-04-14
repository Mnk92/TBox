using System;
using System.Collections.Generic;
using System.IO;
using Mnk.Library.ScriptEngine.Core;
using Mnk.TBox.Tests.Common;
using Mnk.TBox.Tools.SkyNet.Common;
using NUnit.Framework;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Tests.PlugingsShared.SkyNet
{
    [TestFixture]
    [Category("Integration")]
    class When_using_script_runner : ScriptsFixture
    {
        public static string[] Files
        {
            get { return Directory.GetFiles("../../../bin/"+Shared.CompileMode+"/Data/SkyNet/", "*.cs", SearchOption.TopDirectoryOnly); }
        }

        [Test]
        public void Should_get_packages_for_all_exist_scripts([ValueSource("Files")] string path)
        {
            //Arrange
            var sc = new ScriptCompiler<ISkyScript>();

            //Act
            ScriptPackage package = null;
            try
            {
                package = sc.GetPackages(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }

            //Assert
            Assert.IsNotNull(package);
        }

        [Test]
        public void Should_compile_all_exist_scripts([ValueSource("Files")] string path)
        {
            //Arrange
            var sc = new ScriptCompiler<ISkyScript>();

            //Act & Assert
            Assert.IsNotNull(sc.Compile(File.ReadAllText(path), new List<Parameter>()));
        }
    }
}
