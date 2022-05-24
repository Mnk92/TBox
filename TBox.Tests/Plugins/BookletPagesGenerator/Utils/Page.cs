namespace Mnk.TBox.Tests.Plugins.BookletPagesGenerator.Utils
{
    sealed class Page
    {
        public readonly int[] Front;
        public readonly int[] Back;
        public Page(int count)
        {
            Front = Create(count);
            Back = Create(count);
        }
        private static int[] Create(int count)
        {
            return new int[count];
        }
    }
}
