using Mnk.TBox.Plugins.BookletPagesGenerator.Code;
using Mnk.TBox.Tests.Plugins.BookletPagesGenerator.Utils;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Plugins.BookletPagesGenerator
{
	[TestFixture]
	public class When_using_pages_generator
	{
		private const int PagesForPage = 2;

		[Test]
		public void Should_calculate_pages([Range(0, 128, 4)]int count)
		{
			var generator = new PageGenerator(PagesForPage);
			var generatedPages = generator.Calc(count);
			var book = new FakeBook(count, PagesForPage);

			var pages = book.Pages.Length;
			for (var i = 0; i < book.Pages.Length; ++i)
			{
				Assert.AreEqual(book.Pages[i].m_front[0], generatedPages.Front[2 * i]);
				Assert.AreEqual(book.Pages[i].m_front[1], generatedPages.Front[2 * i + 1]);

				Assert.AreEqual(book.Pages[pages - 1 - i].m_back[0], generatedPages.Back[2 * i]);
				Assert.AreEqual(book.Pages[pages - 1 - i].m_back[1], generatedPages.Back[2 * i + 1]);
			}
		}
	}
}
