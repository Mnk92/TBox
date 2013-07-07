using System.ServiceModel;

namespace Mnk.TBox.Plugins.Market.Interfaces.Contracts
{
	[MessageContract]
	public class PluginUploadContract : DataContract
	{
		[MessageHeader(MustUnderstand = true)]
		public Plugin Item;
	}
}
