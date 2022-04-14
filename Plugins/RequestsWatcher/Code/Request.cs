using System;
using Mnk.Library.WpfControls.Components.Drawings.DirectionsTable;

namespace Mnk.TBox.Plugins.RequestsWatcher.Code
{
	[Serializable]
	public class Request : IDirectionable
	{
		public string From { get; set; }
		public string To { get; set; }
		public string Title { get; set; }
		public string Send { get; set; }
		public string Receive { get; set; }

		public Request()
		{
			Send = string.Empty;
			Receive = string.Empty;
		}

		public object Clone()
		{
			return new Request
				{
					From = From, 
					To = To,
					Title = Title,
					Send = Send,
					Receive = Receive
				};
		}
	}
}
