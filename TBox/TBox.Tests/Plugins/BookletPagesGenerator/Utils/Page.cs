namespace Mnk.TBox.Tests.Plugins.BookletPagesGenerator.Utils
{
	sealed class Page
	{
		public int[] m_front;
		public int[] m_back;
		public Page(int count)
		{
			m_front = Create(count);
			m_back = Create(count);
		}
		private static int[] Create(int count)
		{
			return new int[count];
		}
	}
}
