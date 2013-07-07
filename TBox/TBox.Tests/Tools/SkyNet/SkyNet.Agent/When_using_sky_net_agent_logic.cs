using System;
using System.Net;
using Mnk.Library.Common.Communications;
using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Common;
using NUnit.Framework;
using Rhino.Mocks;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Agent
{
    [TestFixture]
    class When_using_sky_net_agent_logic
    {
        private IWorker worker;
        private IHttpContextHelper contextHelper;
        private ISkyNetAgentLogic logic;

        [SetUp]
        public void SetUp()
        {
            worker = MockRepository.GenerateStrictMock<IWorker>();
            contextHelper = MockRepository.GenerateStrictMock<IHttpContextHelper>();
            logic = new SkyNetAgentLogic(worker, contextHelper);
        }

        [TearDown]
        public void TearDown()
        {
            worker.VerifyAllExpectations();
            contextHelper.VerifyAllExpectations();
        }

        [Test]
        public void Should_add_task()
        {
            //Arrange
            var task = new AgentTask();
            worker.Stub(x => x.Start(task));

            //Act
            var id = logic.AddTask(task);

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
            worker.Stub(x => x.Start(task));
            worker.Stub(x => x.IsDone).Return(false);
            contextHelper.Stub(x => x.SetStatusCode(HttpStatusCode.Conflict));

            //Act
            logic.AddTask(task);
            var id = logic.AddTask(task);

            //Assert
            Assert.AreEqual(string.Empty, id);
        }

        [Test]
        public void Shouldnt_get_task_if_no_tasks()
        {
            //Arrange
            contextHelper.Stub(x => x.SetStatusCode(HttpStatusCode.NotFound));

            //Act
            var task = logic.GetTask("abc");

            //Assert
            Assert.IsNull(task);
        }

        [Test]
        public void Should_get_task_if_exist()
        {
            //Arrange
            var task = new AgentTask();
            worker.Stub(x => x.Start(task));
            logic.AddTask(task);

            //Act
            var exist = logic.GetTask(task.Id);

            //Assert
            Assert.AreEqual(task, exist);
        }

        [Test]
        public void Shouldnt_get_current_task_if_no_tasks()
        {
            //Act
            var task = logic.GetCurrentTask();

            //Assert
            Assert.IsNull(task);
        }

        [Test]
        public void Should_get_current_task_if_exist()
        {
            //Arrange
            var task = new AgentTask();
            worker.Stub(x => x.Start(task));
            worker.Stub(x => x.IsDone).Return(true);
            logic.AddTask(task);

            //Act
            var exist = logic.GetCurrentTask();

            //Assert
            Assert.AreEqual(true, exist.IsDone);
            Assert.AreEqual(0, exist.Progress);
        }

        [Test]
        public void Shouldnt_delete_task_if_no_tasks()
        {
            //Arrange
            contextHelper.Stub(x => x.SetStatusCode(HttpStatusCode.NotFound));

            //Act
            var report = logic.DeleteTask("abc");

            //Assert
            Assert.AreEqual(string.Empty, report);
        }

        [Test]
        public void Shouldnt_delete_task_if_current_other_task()
        {
            //Arrange
            var task = new AgentTask();
            worker.Stub(x => x.Start(task));
            logic.AddTask(task);
            contextHelper.Stub(x => x.SetStatusCode(HttpStatusCode.NotFound));

            //Act
            var report = logic.DeleteTask("abc");

            //Assert
            Assert.AreEqual(string.Empty, report);
        }

        [Test]
        public void Shouldnt_delete_task_if_task_not_done()
        {
            //Arrange
            var task = new AgentTask();
            worker.Stub(x => x.Start(task));
            worker.Stub(x => x.IsDone).Return(false);
            logic.AddTask(task);
            contextHelper.Stub(x => x.SetStatusCode(HttpStatusCode.Conflict));

            //Act
            var report = logic.DeleteTask(task.Id);

            //Assert
            Assert.AreEqual(string.Empty, report);
        }

        [Test]
        public void Should_delete_task_if_exist()
        {
            //Arrange
            var task = new AgentTask();
            worker.Stub(x => x.Start(task));
            worker.Stub(x => x.IsDone).Return(true);
            logic.AddTask(task);

            //Act
            var report = logic.DeleteTask(task.Id);

            //Assert
            Assert.AreEqual(task.Report, report);
        }
    }
}
