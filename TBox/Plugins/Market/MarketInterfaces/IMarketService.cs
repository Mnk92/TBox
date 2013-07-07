using System.ServiceModel;
using Mnk.TBox.Plugins.Market.Interfaces.Contracts;

namespace Mnk.TBox.Plugins.Market.Interfaces
{
	[ServiceContract]
	public interface IMarketService
	{
		#region Plugin
		[OperationContract]
		Plugin[] Plugin_GetList(Plugin filter, int offset, int count, bool? onlyPlugins);

		[OperationContract]
		int Plugin_GetListCount(Plugin filter);

		[OperationContract]
		DataContract Plugin_Download(DownloadContract body);

		[OperationContract]
		UploadContract Plugin_Upload( PluginUploadContract body );

		[OperationContract]
		UploadContract Plugin_Upgrade( PluginUploadContract body );

		[OperationContract]
		bool Plugin_Delete( Plugin plugin );

		[OperationContract]
		bool Plugin_Exist( Plugin plugin );
		#endregion Plugin


		#region Bug
		[OperationContract]
		Bug[] Bug_GetList( long uid, int offset, int count );

		[OperationContract]
		int Bug_GetListCount( long uid );

		[OperationContract]
		void Bug_Send(Bug bug);
		#endregion Bug



		#region Author
		[OperationContract]
		string[] Author_GetList();
		#endregion



		#region Type
		[OperationContract]
		string[] Type_GetList();
		#endregion

	}
}
