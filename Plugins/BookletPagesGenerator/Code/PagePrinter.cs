using System.Collections.Generic;
using System.Linq;

namespace Mnk.TBox.Plugins.BookletPagesGenerator.Code
{
	sealed class PagePrinter
	{
		private readonly PageGenerator generator;

		public PagePrinter(int pagesForPage)
		{
			generator = new PageGenerator(pagesForPage);
		}

		public Result Calc(int pageOffset, int pageCount, int pageToPrintForOnce)
		{
			return CanCalcPages(pageToPrintForOnce) ? CalcPages(pageOffset, pageCount, pageToPrintForOnce) : new Result();
		}

		private static bool CanCalcPages(int pageToPrintForOnce)
		{
			return pageToPrintForOnce > 0;
		}

		private Result CalcPages(int pageOffset, int pageCount, int pageToPrintForOnce)
		{
			var result = new Result();
			var j = 0;
			var count = (pageCount - pageOffset) / pageToPrintForOnce;
			var extraCount = (pageCount - pageOffset) - count * pageToPrintForOnce;
			var pages = generator.Calc(pageToPrintForOnce);
			if (count > 0)
			{
				result.Pages = new string[2 * (count + ((extraCount > 0) ? 1 : 0))];
				result.Numbers = new int[result.Pages.Length][];
				for (var i = 0; i < count; i++)
				{
					AppendValuesFromGenerator(ref j, pages, result, pageOffset);
					pageOffset += pageToPrintForOnce;
				}
			}
			if (extraCount > 0)
			{
				generator.Calc(extraCount);
				AppendValuesFromGenerator(ref j, pages, result, pageOffset);
			}
			return result;
		}

		private void AppendValuesFromGenerator(ref int j, Pages pages, Result result, int pageOffset)
		{
			ApplyPages(ref j, result, Clone(pages.Front, pageOffset));
			ApplyPages(ref j, result, Clone(pages.Back, pageOffset));
		}

		private static void ApplyPages(ref int j, Result result, int[] numbers)
		{
			result.Numbers[j] = numbers;
			result.Pages[j++] = PrintNumbers(numbers);
		}

		private static string PrintNumbers(IEnumerable<int> numbers)
		{
			return string.Join(",", numbers);
		}

		private static int[] Clone(IEnumerable<int> arr, int offset)
		{
			return arr.Select(x => Pages.IsValidPage(x) ? (x + offset) : x).ToArray();
		}
	}
}
