using NUnit.Framework;
using Mnk.TBox.Core.PluginsShared.Encoders;

namespace Mnk.TBox.Tests.PluginsShared
{
    [TestFixture]
    class WhenUsingBracketsFormatter
    {
        [Test]
        [TestCase("", "")]
        [TestCase("abc", "abc")]
        [TestCase("1", "1")]
        [TestCase("{", "{")]
        [TestCase("}", "}")]
        [TestCase("{}", "{\r\n}")]
        [TestCase("{int x;}", "{\r\n\tint x;\r\n}")]
        [TestCase("{{}}", "{\r\n\t{\r\n\t}\r\n}")]
        [TestCase("{abc}", "{\r\n\tabc\r\n}")]
        [TestCase("{\"{\"}", "{\r\n\t\"{\"\r\n}")]
        [TestCase("{\"}\"}", "{\r\n\t\"}\"\r\n}")]
        [TestCase("{\"{}\"}", "{\r\n\t\"{}\"\r\n}")]
        [TestCase("{}{}", "{\r\n}\r\n{\r\n}")]
        [TestCase("void main(){}", "void main()\r\n{\r\n}")]
        [TestCase("void main(){printf(\"hello world!\");}", "void main()\r\n{\r\n\tprintf(\"hello world!\");\r\n}")]
        [TestCase("   {   }   ", "{\r\n}")]
        [TestCase("   { abc  ;   {   }  }   ", "{\r\n\tabc  ;\r\n\t{\r\n\t}\r\n}")]
        [TestCase("{{}{}}", "{\r\n\t{\r\n\t}\r\n\t{\r\n\t}\r\n}")]
        [TestCase("{{a}b{c}}", "{\r\n\t{\r\n\t\ta\r\n\t}\r\n\tb\r\n\t{\r\n\t\tc\r\n\t}\r\n}")]
        public void Should_format_string_by_brackets(string value, string expected)
        {
            var bf = new BracketsFormatter();
            Assert.AreEqual(expected, bf.Format(value));
        }
    }
}
