using System;
using Mnk.TBox.Plugins.BookletPagesGenerator.Code;
using Mnk.Library.Common.SaveLoad;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Plugins.BookletPagesGenerator
{
	[TestFixture]
	class When_using_config
	{
		[Test]
		public void Should_set_valid_defaults()
		{
			var config = new Config();

			Assert.AreEqual(0, config.PagesOffset, "PagesOffset");
			Assert.AreEqual(40, config.PagesToPrint, "PagesToPrint");
			Assert.AreEqual(0, config.PrintPageId, "PrintPageId");
			Assert.AreEqual(800, config.TotalPages, "TotalPages");
		}

		[Test]
		public void Should_be_serializable()
		{
			var config = new Config();
			var ser = new ConfigurationSerializer<Config>("temp.xml");

			ser.Save(config);
		}
	}
}
