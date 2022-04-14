namespace Mnk.TBox.Plugins.LocalizationTool.Code.Parsers
{
	class SearchResult
	{
		public bool IsSuccess { get; private set; }
		public string Value { get; private set; }
		public string TextToReplace { get; private set; }
		public string Ident { get; private set; }

		public SearchResult()
		{
			IsSuccess = false;
		}

		public SearchResult(string value, string textToReplace, string ident)
		{
			IsSuccess = true;
			Value = value;
			TextToReplace = textToReplace;
			Ident = ident;
		}
	}
}
