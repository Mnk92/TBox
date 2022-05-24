using Mnk.Library.Common.MT;
using Mnk.TBox.Plugins.TeamManager.Code.Emails;
using Mnk.TBox.Plugins.TeamManager.Code.Emails.Senders;
using Moq;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Plugins.TeamManager.Code.Email
{
    [TestFixture]
    class WhenSendEmails
    {
        private Mock<IReportContext> context;
        private Mock<IUpdater> updater;

        [SetUp]
        public void SetUp()
        {
            context = new Mock<IReportContext>();
            updater = new Mock<IUpdater>();
        }

        [Test]
        public void Should_works_if_no_senders()
        {
            //Arrange
            var sender = new EmailsSender(context.Object);

            //Act
            sender.Send(updater.Object);
        }

        [Test]
        public void Should_provide_valid_args_for_one_sender()
        {
            //Arrange
            var s = new Mock<IReportsSender>();
            s.Setup(x => x.Send(It.Is<IReportContext>(y => y == context.Object), It.Is<IUpdater>(y => y == updater.Object)));
            var sender = new EmailsSender(context.Object, s.Object);

            //Act
            sender.Send(updater.Object);

            //Assert
            s.Verify();
        }

        [Test]
        public void Should_provide_valid_args_for_many_senders()
        {
            //Arrange
            var list = new List<Mock<IReportsSender>>();
            for (var i = 0; i < 10; ++i)
            {
                var s = new Mock<IReportsSender>();
                s.Setup(x => x.Send(It.Is<IReportContext>(y => y == context.Object), It.Is<IUpdater>(y => y == updater.Object)));
            }
            var sender = new EmailsSender(context.Object, list.Select(x => x.Object).ToArray());

            //Act
            sender.Send(updater.Object);

            //Assert
            foreach (var s in list)
            {
                s.Verify();
            }
        }

    }
}
