using System.Collections.Generic;
using System.Linq;

namespace Mnk.TBox.Plugins.BookletPagesGenerator.Code
{
	sealed class Pages
	{
		private const int NoPage = -1;
		public int PagesCount { get; private set; }
		public int SubPagesCount { get; private set; }
		public IList<int> Front { get; private set; }
		public IList<int> Back { get; private set; }

		public Pages(int pageCount, int pagesForPage)
		{
			PagesCount = pageCount;
			SubPagesCount = CalcNeededPages(pageCount, pagesForPage);
			Front = Create(SubPagesCount);
			Back = Create(SubPagesCount);
		}

		private static int CalcNeededPages(int pageCount, int pagesForPage)
		{
			var delta = 2 * pagesForPage;
			var pages = delta * (pageCount / delta);
			if (pages != pageCount)
			{
				pages += delta;
			}
			return pages / 2;
		}

		private static int[] Create(int count)
		{
			return Enumerable.Range(0, count).Select(x => NoPage).ToArray();
		}

		public static bool IsValidPage(int no) { return no != NoPage; }
	}
}
