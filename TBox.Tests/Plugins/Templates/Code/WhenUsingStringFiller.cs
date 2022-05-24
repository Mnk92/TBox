using NUnit.Framework;
using Mnk.TBox.Core.PluginsShared.Templates;

namespace Mnk.TBox.Tests.Plugins.Templates.Code
{
    [TestFixture]
    class WhenUsingStringFiller
    {
        private StringFiller stringFiller;
        [SetUp]
        public void SetUp()
        {
            var dict = new Dictionary<string, string>
            {
                ["$(a)"] = "a",
                ["$(b)"] = "b",
                ["$(c)"] = "c"
            };
            stringFiller = new StringFiller(dict);
        }

        [Test]
        [TestCase("", false)]
        [TestCase("x", false)]
        [TestCase("$(d)", false)]
        [TestCase("$(a)", true)]
        [TestCase("$(a)$(d)", true)]
        [TestCase("x$(a)", true)]
        [TestCase("$(a)x", true)]
        [TestCase("$(a)$(b)", true)]
        [TestCase("x$(a)y$(b)z", true)]
        [TestCase("$(a)$(b)$(c)", true)]
        [TestCase("sample$(a)text$(b)test$(c)fix", true)]
        public void Should_return_valid_can_fill(string template, bool expected)
        {
            //Act
            var actual = stringFiller.CanFill(template);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("", "")]
        [TestCase("x", "x")]
        [TestCase("$(a)", "a")]
        [TestCase("x$(a)", "xa")]
        [TestCase("$(a)x", "ax")]
        [TestCase("$(a)$(b)", "ab")]
        [TestCase("x$(a)y$(b)z", "xaybz")]
        [TestCase("$(a)$(b)$(c)", "abc")]
        [TestCase("sample$(a)text$(b)test$(c)fix", "sampleatextbtestcfix")]
        public void Should_fill_values(string template, string expected)
        {
            //Act
            var actual = stringFiller.Fill(template);

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
