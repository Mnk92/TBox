using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;
using Moq;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Server
{
    using NUnit.Framework;

    [TestFixture]
    class SkyAgentLogicTestFixture
    {
        private string agentData;
        private ServerAgent agent;
        private AgentTask agentTask;
        private ServerTask serverTask;
        private Mock<IAgentsCache> cache;
        private Mock<ISkyNetAgentService.ISkyNetAgentServiceClient> agentService;
        private ISkyAgentLogic agentLogic;

        [SetUp]
        public void SetUp()
        {
            agentData = "DATA";
            serverTask = new ServerTask
            {
                Script = "SCRIPT",
                ZipPackageId = "ZipPackageId"
            };
            agentTask = new AgentTask
            {
                Id = "AgentId",
                IsDone = false
            };
            agent = new ServerAgent();
            cache = new Mock<IAgentsCache>();
            agentService = new Mock<ISkyNetAgentService.ISkyNetAgentServiceClient>();
            cache.Setup(x => x.Get(agent)).Returns(agentService.Object);
            agentLogic = new SkyAgentLogic(cache.Object);
        }

        [TearDown]
        public void TearDown()
        {
            agentService.Verify();
        }

        [Test]
        public void Should_create_worker_task()
        {
            //Arrange
            var id = "ID";
            agentService.Setup(x => x.AddAgentTask(It.Is<AgentTask>(a => Check(a)), new CallOptions()))
                .Returns(new IdMessage { Id = id });

            //Act
            var wt = agentLogic.CreateWorkerTask(agent, agentData, serverTask).Result;

            //Assert
            Assert.IsNotNull(wt);
            Assert.AreEqual(agentData, wt.Config, "Invalid agent data");
            Assert.AreEqual(agent, wt.Agent, "Invalid agent ");
            Assert.IsNotNull(wt.Task, "Invalid task");
            Assert.IsFalse(wt.IsFailed);
        }

        [Test]
        public void Should_handle_error_on_create_worker_task()
        {
            //Arrange
            var ex = new Exception();
            agentService.Setup(x => x.AddAgentTask(It.Is<AgentTask>(a => Check(a)), new CallOptions()))
                .Throws(ex);

            //Act
            var wt = agentLogic.CreateWorkerTask(agent, agentData, serverTask).Result;

            //Assert
            Assert.IsNotNull(wt);
            Assert.AreEqual(ex, wt.Exception, "Invalid exception");
            Assert.IsTrue(wt.IsFailed);
        }

        [Test]
        public void Should_get_task()
        {
            //Arrange
            var wt = new WorkerTask { Task = agentTask, Agent = agent };
            agentService.Setup(x => x.GetAgentTask(new IdMessage { Id = agentTask.Id }, new CallOptions()))
                .Returns(agentTask);

            //Act
            var actual = agentLogic.GetTask(wt);

            //Assert
            Assert.AreEqual(agentTask, actual);
        }

        [Test]
        public void Should_handle_get_task_if_failed()
        {
            //Arrange
            var wt = new WorkerTask { Task = agentTask, Agent = agent, Exception = new Exception() };
            agentService.Setup(x => x.GetAgentTask(new IdMessage { Id = agentTask.Id }, new CallOptions()))
                .Returns(agentTask);

            //Act
            var actual = agentLogic.GetTask(wt).Result;

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void Should_handle_error_on_check_get_task()
        {
            //Arrange
            var ex = new Exception();
            var wt = new WorkerTask { Task = agentTask, Agent = agent };
            agentService.Setup(x => x.GetAgentTask(new IdMessage { Id = agentTask.Id }, new CallOptions()))
                .Throws(ex);

            //Act
            var actual = agentLogic.GetTask(wt).Result;

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void Should_build_report()
        {
            //Arrange
            var report = "REPORT";
            var wt = new WorkerTask { Task = agentTask, Agent = agent };
            agentService.Setup(x => x.DeleteAgentTask(new IdMessage { Id = agentTask.Id }, new CallOptions()))
                .Returns(new ReportMessage { Report = report });

            //Act
            var actual = agentLogic.BuildReport(wt).Result;

            //Assert
            Assert.AreEqual(wt.Config, actual.Config);
            Assert.AreEqual(wt.Agent, actual.Agent);
            Assert.AreEqual(report, actual.Report);
        }

        [Test]
        public void Should_build_report_for_failed_task()
        {
            //Arrange
            var wt = new WorkerTask { Task = agentTask, Agent = agent, Exception = new Exception() };

            //Act
            var actual = agentLogic.BuildReport(wt).Result;

            //Assert
            Assert.AreEqual(wt.Config, actual.Config);
            Assert.AreEqual(wt.Agent, actual.Agent);
            Assert.AreEqual(wt.Exception.ToString(), actual.Report);
        }

        [Test]
        public void Should_handle_error_on_build_report()
        {
            //Arrange
            var ex = new Exception();
            var wt = new WorkerTask { Task = agentTask, Agent = agent };
            agentService.Setup(x => x.DeleteAgentTask(new IdMessage { Id = agentTask.Id }, new CallOptions()))
                .Throws(ex);

            //Act
            var actual = agentLogic.BuildReport(wt).Result;

            //Assert
            Assert.AreEqual(wt.Config, actual.Config);
            Assert.AreEqual(wt.Agent, actual.Agent);
            Assert.AreEqual(ex.ToString(), actual.Report);
        }

        [Test]
        public void Should_check_is_alive()
        {
            //Arrange
            agentService.Setup(x => x.PingIsAgentAlive(new Empty(), new CallOptions()))
                .Returns(new Empty());

            //Act
            var actual = agentLogic.IsAlive(agent).Result;

            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void Should_check_is_alive_if_exception()
        {
            //Arrange
            agentService.Setup(x => x.PingIsAgentAlive(new Empty(), new CallOptions()))
                .Throws(new Exception());

            //Act
            var actual = agentLogic.IsAlive(agent).Result;

            //Assert
            Assert.IsFalse(actual);
        }

        private bool Check(AgentTask task)
        {
            Assert.AreEqual(agentData, task.Config, "Invalid agent data");
            Assert.AreEqual(serverTask.Script, task.Script, "Invalid agent data");
            Assert.AreEqual(serverTask.ZipPackageId, task.ZipPackageId, "Invalid zip id");
            return true;
        }
    }

}
