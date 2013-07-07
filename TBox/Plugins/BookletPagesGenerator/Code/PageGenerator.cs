using System.Collections.Generic;

namespace Mnk.TBox.Plugins.BookletPagesGenerator.Code
{
	sealed class PageGenerator
	{
		private readonly int pagesForPage;

		public PageGenerator(int pagesForPage)
		{
			this.pagesForPage = pagesForPage;
		}

		public Pages Calc(int pageCount)
		{
			var pages = new Pages(pageCount, pagesForPage);
			var count = pages.SubPagesCount / 2;
			for (var i = 0; i < count; i++)
			{
				InitPage(pages, i, count);
			}
			return pages;
		}

		private void InitPage(Pages pages, int pageNo, int count)
		{
			var id = pageNo * pagesForPage;
			var i = 2 * pageNo;
			InitPage(pages, pages.Front, id + 1, i);
			InitPage(pages, pages.Back, count * pagesForPage - 1 - (id + 1), i + 1);

			id = (count - 1 - pageNo) * pagesForPage;
			i = 2 * (count + pageNo);
			InitPage(pages, pages.Back, count * pagesForPage - 1 - (id), i);
			InitPage(pages, pages.Front, id, i + 1);
		}

		private static void InitPage(Pages pages, IList<int> arr, int id, int pageNo)
		{
			if (IsMorePages(pages, pageNo))
			{
				arr[id] = pageNo + 1;
			}
		}

		private static bool IsMorePages(Pages pages, int id)
		{
			return id < pages.PagesCount;
		}
	}
}
