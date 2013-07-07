using System;
using System.Collections.ObjectModel;
using Mnk.Library.Common.Network;

namespace Mnk.TBox.Plugins.Requestor.Code.Settings
{
	[Serializable]
	public sealed class RequestConfig : ICloneable
	{
		public string Url { get; set; }
		public HttpMethod Method { get; set; }

		public string Body { get; set; }
		public ObservableCollection<Header> Headers { get; set; }

		public RequestConfig()
		{
			Url = @"http://google.com";
			Method = HttpMethod.GET;
			Headers = new ObservableCollection<Header>();
		}

		public object Clone()
		{
			var c = new RequestConfig
			{
				Url = Url,
				Body = Body,
				Headers = new ObservableCollection<Header>(Headers),
				Method = Method,
			};
			return c;
		}
	}
}
