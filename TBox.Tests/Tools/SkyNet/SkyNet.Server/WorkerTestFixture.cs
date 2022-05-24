using System.IO;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;
using Moq;
using NUnit.Framework;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Server
{
    [TestFixture]
    class WorkerTestFixture
    {
        private Mock<IScriptCompiler<ISkyScript>> compiler;
        private Mock<ISkyAgentLogic> agentLogic;
        private Mock<IDataPacker> dataPacker;
        private Mock<ISkyNetFileService.ISkyNetFileServiceClient> skyNetFileService;
        private IWorker worker;
        private ServerAgent[] serverAgents;
        private ServerTask serverTask;
        private Mock<ISkyScript> script;

        [SetUp]
        public void SetUp()
        {
            compiler = new Mock<IScriptCompiler<ISkyScript>>();
            agentLogic = new Mock<ISkyAgentLogic>();
            dataPacker = new Mock<IDataPacker>();
            skyNetFileService = new Mock<ISkyNetFileService.ISkyNetFileServiceClient>();
            worker = new Worker(compiler.Object, agentLogic.Object, dataPacker.Object, skyNetFileService.Object);
            serverAgents = new[] { new ServerAgent { } };
            serverTask = new ServerTask
            {
                ScriptParameters = "[]",
                Script = "Script",
                ZipPackageId = "ZipPackageId"
            };
            script = new Mock<ISkyScript>();
            compiler.Setup(x => x.Compile(serverTask.Script, Array.Empty<Parameter>())).Returns(script.Object);
        }

        [TearDown]
        public void TearDown()
        {
            compiler.Verify();
            agentLogic.Verify();
            dataPacker.Verify();
            skyNetFileService.Verify();
        }

        [Test, Ignore("Fix needed")]
        public void Should_process_task()
        {
            //Arrange
            agentLogic.Setup(x => x.IsAlive(serverAgents[0]))
                .Returns(Task.FromResult(true));
            var saw = new[] {new SkyAgentWork
            {
                Agent = serverAgents[0],
                Config = "AGENTCONFIG",
                Report = "REPORT"
            }};
            var path = "";
            var s = new MemoryStream();
            // todo: fix
            //skyNetFileService.Setup(x => x.Download(new IdMessage { Id = serverTask.ZipPackageId })).Returns(s);
            dataPacker.Setup(x => x.Unpack(s)).Returns(path);
            script.Setup(x => x.ServerBuildAgentsData(path, serverAgents))
                .Returns(saw);
            var wt = new WorkerTask
            {
                Agent = serverAgents[0],
                Task = new AgentTask
                {
                    Config = saw[0].Config,
                    Script = serverTask.Script,
                    ZipPackageId = serverTask.ZipPackageId
                }
            };
            agentLogic.Setup(x => x.CreateWorkerTask(serverAgents[0], saw[0].Config, serverTask))
                .Returns(Task.FromResult(wt));

            var task = new AgentTask { IsDone = true };
            agentLogic.Setup(x => x.GetTask(wt))
                .Returns(Task.FromResult(task));

            agentLogic.Setup(x => x.BuildReport(wt))
                .Returns(Task.FromResult(saw[0]));

            script.Setup(x => x.ServerBuildResultByAgentResults(saw))
                .Returns("FULLREPORT");

            //Act
            worker.ProcessTask(serverTask, serverAgents);

            //Assert
            Assert.AreEqual("FULLREPORT", serverTask.Report);

        }
    }
}
