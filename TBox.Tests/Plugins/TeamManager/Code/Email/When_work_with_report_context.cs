using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.TBox.Plugins.TeamManager.Code;
using Mnk.TBox.Plugins.TeamManager.Code.Emails;
using Mnk.TBox.Plugins.TeamManager.Code.Reports.Contracts;
using Mnk.TBox.Plugins.TeamManager.Code.Settings;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Plugins.TeamManager.Code.Email
{
    [TestFixture]
    class When_work_with_report_context
    {
        private Profile profile;
        private Func<IList<ReportPerson>, string> reportBuilder;
        private FullReport fullReport;
        private IList<ReportPerson> allPersons;
        private IReportContext context;

        [SetUp]
        public void SetUp()
        {
            profile = new Profile();
            reportBuilder = i=>"report " + i.Count;
            fullReport = new FullReport();
            allPersons = new List<ReportPerson>();
            context = new ReportContext(fullReport, allPersons, profile, reportBuilder);
        }

        [Test]
        public void Should_provide_report_config()
        {
            // Assert
            Assert.AreEqual(fullReport, context.FullReport);
        }

        [Test]
        public void Should_provide_persons()
        {
            // Assert
            Assert.AreEqual(allPersons, context.AllPersons);
        }

        [Test]
        public void Should_create_smtp_email_sender()
        {
            //Arrange
            profile.Email = new TBox.Plugins.TeamManager.Code.Settings.Email{IsSmtp = true};
            context = new ReportContext(fullReport, allPersons, profile, reportBuilder);

            // Assert
            Assert.AreEqual(typeof(SmptEmailSender), context.Sender.GetType());
        }

        [Test]
        public void Should_create_exchange_email_sender()
        {
            //Arrange
            profile.Email = new TBox.Plugins.TeamManager.Code.Settings.Email { IsSmtp = false };
            context = new ReportContext(fullReport, allPersons, profile, reportBuilder);

            // Assert
            Assert.AreEqual(typeof(ExchangeEmailSender), context.Sender.GetType());
        }

        [Test]
        public void Should_build_report()
        {
            //Assert
            Assert.AreEqual("report 1", context.BuildReport(new []{new ReportPerson()}));
        }

        public static IEnumerable<Person> Persons = new[]
            {
                new Person { ReportType = (int)TimeReportType.Personal, Key = "Personal"},
                new Person { ReportType = (int)TimeReportType.Team, Key = "Team" },
                new Person { ReportType = (int)TimeReportType.Full, Key = "Full" },
                new Person { ReportType = (int)(TimeReportType.Full | TimeReportType.Team | TimeReportType.Personal), Key = "Mix" }
            };

        [Test]
        public void Should_filter_persons_by_personal()
        {
            //Arrange
            profile.Persons = new CheckableDataCollection<Person>(Persons);

            //Assert
            CollectionAssert.AreEqual(new[]{"Personal", "Mix"}, context.GetPersons(TimeReportType.Personal).Select(x=>x.Key).ToArray());
        }

        [Test]
        public void Should_filter_persons_by_team()
        {
            //Arrange
            profile.Persons = new CheckableDataCollection<Person>(Persons);

            //Assert
            CollectionAssert.AreEqual(new[] { "Team", "Mix" }, context.GetPersons(TimeReportType.Team).Select(x => x.Key).ToArray());
        }


        [Test]
        public void Should_filter_persons_by_full()
        {
            //Arrange
            profile.Persons = new CheckableDataCollection<Person>(Persons);

            //Assert
            CollectionAssert.AreEqual(new[] { "Full", "Mix" }, context.GetPersons(TimeReportType.Full).Select(x => x.Key).ToArray());
        }
    }
}

