using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [ServiceContract]
    public interface ISkyNetServerAgentsService
    {
        [OperationContract]
        [WebGet(UriTemplate = "agent/all", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IList<ServerAgent> GetAgents();

        [OperationContract]
        [WebInvoke(Method = "CREATE", UriTemplate = "agent", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void ConnectAgent(ServerAgent agent);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "agent", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void DisconnectAgent(string endpoint);

        [OperationContract]
        [WebGet(UriTemplate = "ping", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void PingIsAlive();

    }
}
