using System.Collections.Generic;
using NUnit.Framework;
using Mnk.TBox.Core.PluginsShared.Encoders;

namespace Mnk.TBox.Tests.PlugingsShared
{
	[TestFixture]
	class When_using_common_ops
	{
		[Test]
		[TestCase("", -1)]
		[TestCase("abc", -1)]
		[TestCase("1", -1)]
		[TestCase("{", 0)]
		[TestCase("abc{def", 3)]
		[TestCase("\"{\"", -1)]
		[TestCase("abc\"{\"def}", -1)]
		[TestCase("abc\"{\"def{", 9)]
		[TestCase("\'{\'", -1)]
		[TestCase("abc\'{\'def}", -1)]
		[TestCase("abc\'{\'def{", 9)]
		[TestCase("abc\"{\'def\'{\"", -1)]
		[TestCase("abc\"{\'def\'{\"{", 12)]
		[TestCase("abc\"{\\\"def\\\"{\"{", 14)]
		public void Should_find_first_not_string_char(string value, int expected)
		{
			Assert.AreEqual(expected, value.FindFirstNotString('{', 0));
		}

		[Test]
		[TestCase("", -1)]
		[TestCase("abc", -1)]
		[TestCase("1", -1)]
		[TestCase("}", 0)]
		[TestCase("abc}def", 3)]
		[TestCase("\"}\"", -1)]
		[TestCase("abc\"}\"def{", -1)]
		[TestCase("abc\"}\"def}", 9)]
		[TestCase("\'}\'", -1)]
		[TestCase("abc\'}\'def{", -1)]
		[TestCase("abc\'}\'def}", 9)]
		[TestCase("abc\"}\'def\'}\"", -1)]
		[TestCase("abc\"}\'def\'}\"}", 12)]
		[TestCase("abc\"}\\\"def\\\"}\"}", 14)]
		[TestCase("{abc{abc}abc}}", 13)]
		[TestCase("{abc{abc abc}", -1)]
		public void Should_find_first_not_string_char_and_skip_extra_chars(string value, int expected)
		{
			Assert.AreEqual(expected, value.FindFirstNotString('}', 0, new KeyValuePair<char, char>('{', '}')));
		}
	}
}
