using Mnk.TBox.Tools.SkyNet.Agent;
using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Moq;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Agent
{
    [TestFixture]
    class SkyNetAgentLogicTestsFixture
    {
        private Mock<IWorker> worker;
        private SkyNetAgentService logic;

        [SetUp]
        public void SetUp()
        {
            worker = new Mock<IWorker>();
            logic = new SkyNetAgentService(worker.Object);
        }

        [TearDown]
        public void TearDown()
        {
            worker.Verify();
        }

        [Test]
        public void Should_add_task()
        {
            //Arrange
            var task = new AgentTask();
            worker.Setup(x => x.Start(task));

            //Act
            var id = logic.AddAgentTask(task, null).Result.Id;

            //Assert
            Guid dummy;
            Assert.IsTrue(Guid.TryParse(id, out dummy), "should return guid");
            Assert.AreEqual(id, task.Id, "should set task id");
        }

        [Test]
        public void Shouldnt_add_task_twice()
        {
            //Arrange
            var task = new AgentTask();
            worker.Setup(x => x.Start(task));
            worker.Setup(x => x.IsDone).Returns(false);

            //Act
            logic.AddAgentTask(task, null).Wait();
            var id = logic.AddAgentTask(task, null).Result.Id;

            //Assert
            Assert.AreEqual(string.Empty, id);
        }

        [Test]
        public void Shouldnt_get_task_if_no_tasks()
        {
            //Arrange

            //Act
            var task = logic.GetAgentTask(new IdMessage { Id = "abc" }, null).Result;

            //Assert
            Assert.IsNull(task);
        }

        [Test]
        public void Should_get_task_if_exist()
        {
            //Arrange
            var task = new AgentTask();
            worker.Setup(x => x.Start(task));
            logic.AddAgentTask(task, null).Wait();

            //Act
            var exist = logic.GetAgentTask(new IdMessage { Id = task.Id }, null).Result;

            //Assert
            Assert.AreEqual(task, exist);
        }

        [Test]
        public void Shouldnt_get_current_task_if_no_tasks()
        {
            //Act
            var task = logic.GetCurrentAgentTask(null, null).Result;

            //Assert
            Assert.IsNull(task);
        }

        [Test]
        public void Should_get_current_task_if_exist()
        {
            //Arrange
            var task = new AgentTask();
            worker.Setup(x => x.Start(task));
            worker.Setup(x => x.IsDone).Returns(true);
            logic.AddAgentTask(task, null).Wait();

            //Act
            var exist = logic.GetCurrentAgentTask(null, null).Result;

            //Assert
            Assert.AreEqual(true, exist.IsDone);
            Assert.AreEqual(0, exist.Progress);
        }

        [Test]
        public void Shouldnt_delete_task_if_no_tasks()
        {
            //Arrange

            //Act
            var report = logic.DeleteAgentTask(new IdMessage { Id = "abc" }, null).Result;

            //Assert
            Assert.IsEmpty(report.Report);
        }

        [Test]
        public void Shouldnt_delete_task_if_current_other_task()
        {
            //Arrange
            var task = new AgentTask();
            worker.Setup(x => x.Start(task));
            logic.AddAgentTask(task, null).Wait();

            //Act
            var report = logic.DeleteAgentTask(new IdMessage { Id = "abc" }, null).Result;

            //Assert
            Assert.IsEmpty(report.Report);
        }

        [Test]
        public void Shouldnt_delete_task_if_task_not_done()
        {
            //Arrange
            var task = new AgentTask();
            worker.Setup(x => x.Start(task));
            worker.Setup(x => x.IsDone).Returns(false);
            logic.AddAgentTask(task, null).Wait();

            //Act
            var report = logic.DeleteAgentTask(new IdMessage { Id = task.Id }, null).Result;

            //Assert
            Assert.IsEmpty(report.Report);
        }

        [Test]
        public void Should_delete_task_if_exist()
        {
            //Arrange
            var task = new AgentTask();
            worker.Setup(x => x.Start(task));
            worker.Setup(x => x.IsDone).Returns(true);
            logic.AddAgentTask(task, null).Wait();

            //Act
            var report = logic.DeleteAgentTask(new IdMessage { Id = task.Id }, null).Result;

            //Assert
            Assert.IsEmpty(report.Report);
        }
    }
}
