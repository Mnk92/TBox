using System.ServiceModel;

namespace Mnk.TBox.Plugins.Market.Interfaces.Contracts
{
	[MessageContract]
	public class UploadContract
	{
		[MessageHeader(MustUnderstand = true)]
		public bool Success;

		[MessageHeader(MustUnderstand = true)]
		public bool Exist;
	}
}
