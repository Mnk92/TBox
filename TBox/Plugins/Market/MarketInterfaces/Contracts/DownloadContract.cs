using System.ServiceModel;

namespace Mnk.TBox.Plugins.Market.Interfaces.Contracts
{
	[MessageContract]
	public class DownloadContract
	{
		[MessageHeader(MustUnderstand = true)]
		public string Author;

		[MessageHeader(MustUnderstand = true)]
		public string Name;
	}
}
