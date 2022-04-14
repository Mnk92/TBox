using System.Threading;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Moq;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Agent
{
    [TestFixture]
    class WorkerTestFixture
    {
        [Test, Timeout(50000)]
        public void Should_execute_script()
        {
            //Arrange
            var task = new AgentTask
            {
                ZipPackageId = "ZipPackageId",
                Script = "Script",
                Config = "CONFIG"
            };
            var path = "PATH";
            var skyContext = new Mock<ISkyContext>();
            var script = new Mock<ISkyScript>();
            script.Setup(x => x.AgentExecute(path, task.Config, skyContext.Object));
            var compiler = new Mock<IScriptCompiler<ISkyScript>>();
            compiler.Setup(x => x.Compile(task.Script)).Returns(script.Object);
            var downloader = new Mock<IFilesDownloader>();
            downloader.Setup(x => x.DownloadAndUnpackFiles(task.ZipPackageId)).Returns(path);
            var worker = new Worker(skyContext.Object, compiler.Object, downloader.Object);

            //Act
            worker.Start(task);

            //Assert
            while (!worker.IsDone) { Thread.Sleep(100); }
            downloader.Verify();
            compiler.Verify();
            script.Verify();
        }
    }
}
