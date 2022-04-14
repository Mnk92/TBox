using Mnk.TBox.Plugins.BookletPagesGenerator.Code;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Plugins.BookletPagesGenerator
{
	[TestFixture]
	class When_using_result
	{
		readonly static int[][] Numbers = new[] { new[] { 1, 2, 3 } };
		readonly static string[] Pages = new[] { "a", "b", "c" };

		[Test]
		public void Should_read_and_set_properties()
		{
			var result = new Result { Numbers = Numbers, Pages = Pages };

			Assert.AreEqual(Numbers, result.Numbers, "Numbers");
			Assert.AreEqual(Pages, result.Pages, "Pages");
		}

		[Test]
		public void Should_return_valid_if_not_empty_pages()
		{
			var result = new Result { Pages = Pages };

			Assert.IsTrue(result.IsValid());
		}

		[Test]
		public void Should_return_not_valid_if_empty_pages()
		{
			var result = new Result { Numbers = Numbers };

			Assert.IsFalse(result.IsValid());
		}
	}
}
