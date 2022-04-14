namespace Mnk.TBox.Plugins.BookletPagesGenerator.Code
{
	sealed class Result
	{
		public string[] Pages { get; set; }
		public int[][] Numbers { get; set; }
		public bool IsValid() { return Pages != null; }
	}
}
