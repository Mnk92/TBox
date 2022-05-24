namespace Mnk.TBox.Tests.Plugins.BookletPagesGenerator.Utils
{
    sealed class FakeBook
    {
        public readonly Page[] Pages;
        public FakeBook(int pagesCount, int pagesForPage)
        {
            var count = pagesCount / pagesForPage / 2;
            Pages = new Page[count];
            for (var i = 0; i < count; ++i)
            {
                Pages[i] = new Page(pagesForPage);
            }
            var pageNo = 0;
            for (var i = 0; i < count; ++i)
            {
                Pages[i].Front[1] = NextPage(ref pageNo, pagesCount);
                Pages[i].Back[0] = NextPage(ref pageNo, pagesCount);
            }
            for (var i = count - 1; i >= 0; --i)
            {
                Pages[i].Back[1] = NextPage(ref pageNo, pagesCount);
                Pages[i].Front[0] = NextPage(ref pageNo, pagesCount);
            }
        }

        private static int NextPage(ref int pageNo, int pagesCount)
        {
            ++pageNo;
            return (pageNo > pagesCount) ? -1 : pageNo;
        }
    }
}
