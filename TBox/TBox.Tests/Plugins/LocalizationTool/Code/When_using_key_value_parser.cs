using System;
using Mnk.TBox.Plugins.LocalizationTool.Code.Parsers;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Plugins.LocalizationTool.Code
{
	[TestFixture]
	class When_using_key_value_parser
	{
		private readonly object[] TestData = new object[]
			                            {
				new TestCaseData("$(key)v$(value)test", "KEYvVALUEtest"),
				new TestCaseData("$(key)test$(value)", "KEYtestVALUE"),
				new TestCaseData("test$(key)v$(value)", "testKEYvVALUE"),
				new TestCaseData("$(key)\t$(value)", "KEY\tVALUE"),
				new TestCaseData("\t$(key)\t$(value)\t", "\tKEY\tVALUE\t"),
				new TestCaseData("[$(key)]=[$(value)]", "[KEY]=[VALUE]"),
				new TestCaseData("teststring[$(key)]=[$(value)]", "teststring[KEY]=[VALUE]"),
				new TestCaseData("teststring[\"$(key)\"]=\"$(value)\"", "teststring[\"KEY\"]=\"VALUE\""),
				new TestCaseData("teststring[\"$(key)\"]=\"$(value)\";", "teststring[\"KEY\"]=\"VALUE\";"),
			                            };

		[Test]
		[TestCase("")]
		[TestCase("$(key)")]
		[TestCase("$(key)abc")]
		[TestCase("$(value)")]
		[TestCase("xce$(value)")]
		[TestCase("$(key)$(value)")]
		[TestCase("aa$(key)$(value)bb")]
		[TestCase("$(value)$(key)")]
		public void Should_be_exception_on_incorrect_data(string template)
		{
			Assert.Throws<ArgumentException>(() => new KeyValueParser(template));
		}

		[Test]
		[TestCaseSource("TestData")]
		public void Should_parse_template_and_save_valid_data(string template, string expected)
		{
			//Arrange
			var parser = new KeyValueParser(template);

			//Act
			var actual = parser.Save("KEY", "VALUE");

			//Assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase("$(key)\t$(value)", "abcde")]
		[TestCase("$(key)\t$(value)", "")]
		[TestCase("$(key)\t$(value)]", "key\tvalue")]
		[TestCase("[$(key)\t$(value)", "key\tvalue")]
		public void Should_be_exception_on_load_incorrect_data(string template, string text)
		{
			//Arrange
			var parser = new KeyValueParser(template);

			//Act & Assert
			Assert.Throws<ArgumentException>(() => parser.Load(text));
		}

		[Test]
		[TestCaseSource("TestData")]
		public void Should_parse_template_and_load_valid_data(string template, string text)
		{
			//Arrange
			var parser = new KeyValueParser(template);

			//Act
			var actual = parser.Load(text);

			//Assert
			Assert.AreEqual("KEY", actual.Key);
			Assert.AreEqual("VALUE", actual.Value);
		}
	}
}
