using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mnk.Library.Common.MT;
using Mnk.TBox.Plugins.TeamManager.Code.Emails;
using Mnk.TBox.Plugins.TeamManager.Code.Emails.Senders;
using NUnit.Framework;
using Rhino.Mocks;

namespace Mnk.TBox.Tests.Plugins.TeamManager.Code.Email
{
    [TestFixture]
    class When_send_emails
    {
        private IReportContext context;
        private IUpdater updater;

        [SetUp]
        public void SetUp()
        {
            context = MockRepository.GenerateMock<IReportContext>();
            updater = MockRepository.GenerateMock<IUpdater>();
        }

        [Test]
        public void Should_works_if_no_senders()
        {
            //Arrange
            var sender = new EmailsSender(context);

            //Act
            sender.Send(updater);
        }

        [Test]
        public void Should_provide_valid_args_for_one_sender()
        {
            //Arrange
            var s = MockRepository.GenerateMock<IReportsSender>();
            s.Stub(x=>x.Send(Arg<IReportContext>.Is.Equal(context), Arg<IUpdater>.Is.Equal(updater)));
            var sender = new EmailsSender(context, s);

            //Act
            sender.Send(updater);

            //Assert
            s.VerifyAllExpectations();
        }

        [Test]
        public void Should_provide_valid_args_for_many_senders()
        {
            //Arrange
            var list = new List<IReportsSender>();
            for (var i = 0; i < 10; ++i)
            {
                var s = MockRepository.GenerateMock<IReportsSender>();
                s.Stub(x => x.Send(Arg<IReportContext>.Is.Equal(context), Arg<IUpdater>.Is.Equal(updater)));
            }
            var sender = new EmailsSender(context, list.ToArray());

            //Act
            sender.Send(updater);

            //Assert
            foreach (var s in list)
            {
                s.VerifyAllExpectations();
            }
        }

    }
}
