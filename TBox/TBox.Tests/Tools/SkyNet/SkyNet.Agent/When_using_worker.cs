using System.Threading;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Common;
using NUnit.Framework;
using Rhino.Mocks;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Agent
{
    [TestFixture]
    class When_using_worker
    {
        [Test,Timeout(50000)]
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
            var skyContext = MockRepository.GenerateMock<ISkyContext>();
            var script = MockRepository.GenerateMock<ISkyScript>();
            script.Stub(x => x.AgentExecute(path, task.Config, skyContext));
            var compiler = MockRepository.GenerateMock<IScriptCompiler<ISkyScript>>();
            compiler.Stub(x => x.Compile(task.Script)).Return(script);
            var downloader = MockRepository.GenerateMock<IFilesDownloader>();
            downloader.Stub(x => x.DownloadAndUnpackFiles(task.ZipPackageId)).Return(path);
            var worker = new Worker(skyContext, compiler, downloader);

            //Act
            worker.Start(task);

            //Assert
            while (!worker.IsDone) { Thread.Sleep(100);}
            downloader.VerifyAllExpectations();
            compiler.VerifyAllExpectations();
            script.VerifyAllExpectations();
        }
    }
}
