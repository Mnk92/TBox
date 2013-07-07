using System.IO;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;
using NUnit.Framework;
using Rhino.Mocks;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Server
{
    [TestFixture]
    class When_using_worker
    {
        private IScriptCompiler<ISkyScript> compiler;
        private ISkyAgentLogic agentLogic;
        private IDataPacker dataPacker;
        private ISkyNetFileServiceLogic skyNetFileService;
        private IWorker worker;
        private ServerAgent[] serverAgents;
        private ServerTask serverTask;
        private ISkyScript script;
        
        [SetUp]
        public void SetUp()
        {
            compiler = MockRepository.GenerateStrictMock<IScriptCompiler<ISkyScript>>();
            agentLogic = MockRepository.GenerateStrictMock<ISkyAgentLogic>();
            dataPacker = MockRepository.GenerateStrictMock<IDataPacker>();
            skyNetFileService = MockRepository.GenerateStrictMock<ISkyNetFileServiceLogic>();
            worker = new Worker(compiler,agentLogic, dataPacker, skyNetFileService);
            serverAgents = new[] { new ServerAgent { } };
            serverTask = new ServerTask
            {
                ScriptParameters = "[]",
                Script = "Script",
                ZipPackageId = "ZipPackageId"
            };
            script = MockRepository.GenerateMock<ISkyScript>();
            compiler.Stub(x => x.Compile(serverTask.Script, new Parameter[0])).Return(script);
        }

        [TearDown]
        public void TearDown()
        {
            compiler.VerifyAllExpectations();
            agentLogic.VerifyAllExpectations();
            dataPacker.VerifyAllExpectations();
            skyNetFileService.VerifyAllExpectations();
        }

        [Test]
        public void Should_process_task()
        {
            //Arrange
            agentLogic.Stub(x => x.IsAlive(serverAgents[0]))
                .Return(true);
            var saw = new[] {new SkyAgentWork
            {
                Agent = serverAgents[0], 
                Config = "AGENTCONFIG", 
                Report = "REPORT"
            }};
            var path = "";
            var s = new MemoryStream();
            skyNetFileService.Stub(x => x.Download(serverTask.ZipPackageId)).Return(s);
            dataPacker.Stub(x => x.Unpack(s)).Return(path);
            script.Stub(x => x.ServerBuildAgentsData(path,serverAgents))
                .Return(saw);
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
            agentLogic.Stub(x => x.CreateWorkerTask(serverAgents[0], saw[0].Config, serverTask ))
                .Return(wt);

            var task = new AgentTask{IsDone = true};
            agentLogic.Stub(x => x.GetTask(wt))
                .Return(task);

            agentLogic.Stub(x => x.BuildReport(wt))
                .Return(saw[0]);

            script.Stub(x => x.ServerBuildResultByAgentResults(saw))
                .Return("FULLREPORT");

            //Act
            worker.ProcessTask(serverTask, serverAgents);

            //Assert
            Assert.AreEqual("FULLREPORT", serverTask.Report);

        }
    }
}
