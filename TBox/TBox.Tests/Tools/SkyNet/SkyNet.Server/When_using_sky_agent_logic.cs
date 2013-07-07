using System;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;
using Rhino.Mocks;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Server
{
    using NUnit.Framework;

    [TestFixture]
    class When_using_sky_agent_logic
    {
        private string agentData;
        private ServerAgent agent;
        private AgentTask agentTask;
        private ServerTask serverTask;
        private IAgentsCache cache;
        private ISkyAgentLogic agentLogic;
        private ISkyNetAgentService agentService;

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
            cache = MockRepository.GenerateStrictMock<IAgentsCache>();
            agentService = MockRepository.GenerateStrictMock<ISkyNetAgentService>();
            cache.Stub(x => x.Get(agent)).Return(agentService);
            agentLogic = new SkyAgentLogic(cache);
        }

        [TearDown]
        public void TearDown()
        {
            agentService.VerifyAllExpectations();
        }

        [Test]
        public void Should_create_worker_task()
        {
            //Arrange
            var id = "ID";
            agentService.Stub(x => x.AddTask(Arg<AgentTask>.Matches(a=>Check(a))))
                .Return(id);

            //Act
            var wt = agentLogic.CreateWorkerTask(agent, agentData, serverTask);

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
            agentService.Stub(x => x.AddTask(Arg<AgentTask>.Matches(a => Check(a))))
                .Throw(ex);

            //Act
            var wt = agentLogic.CreateWorkerTask(agent, agentData, serverTask);

            //Assert
            Assert.IsNotNull(wt);
            Assert.AreEqual(ex, wt.Exception, "Invalid exception");
            Assert.IsTrue(wt.IsFailed);
        }

        [Test]
        public void Should_get_task()
        {
            //Arrange
            var wt = new WorkerTask{Task = agentTask, Agent = agent};
            agentService.Stub(x => x.GetTask(agentTask.Id))
                .Return(agentTask);

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
            agentService.Stub(x => x.GetTask(agentTask.Id))
                .Return(agentTask);

            //Act
            var actual = agentLogic.GetTask(wt);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void Should_handle_error_on_check_get_task()
        {
            //Arrange
            var ex = new Exception();
            var wt = new WorkerTask { Task = agentTask, Agent = agent };
            agentService.Stub(x => x.GetTask(agentTask.Id))
                .Throw(ex);

            //Act
            var actual = agentLogic.GetTask(wt);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void Should_build_report()
        {
            //Arrange
            var report = "REPORT";
            var wt = new WorkerTask { Task = agentTask, Agent = agent};
            agentService.Stub(x => x.DeleteTask(agentTask.Id))
                .Return(report);

            //Act
            var actual = agentLogic.BuildReport(wt);

            //Assert
            Assert.AreEqual(wt.Config, actual.Config);
            Assert.AreEqual(wt.Agent, actual.Agent);
            Assert.AreEqual(report, actual.Report);
        }

        [Test]
        public void Should_build_report_for_failed_task()
        {
            //Arrange
            var wt = new WorkerTask { Task = agentTask, Agent = agent, Exception = new Exception()};

            //Act
            var actual = agentLogic.BuildReport(wt);

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
            agentService.Stub(x => x.DeleteTask(agentTask.Id))
                .Throw(ex);

            //Act
            var actual = agentLogic.BuildReport(wt);

            //Assert
            Assert.AreEqual(wt.Config, actual.Config);
            Assert.AreEqual(wt.Agent, actual.Agent);
            Assert.AreEqual(ex.ToString(), actual.Report);
        }

        [Test]
        public void Should_check_is_alive()
        {
            //Arrange
            agentService.Stub(x => x.PingIsAlive());

            //Act
            var actual = agentLogic.IsAlive(agent);

            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void Should_check_is_alive_if_exception()
        {
            //Arrange
            agentService.Stub(x => x.PingIsAlive())
                .Throw(new Exception());

            //Act
            var actual = agentLogic.IsAlive(agent);

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
