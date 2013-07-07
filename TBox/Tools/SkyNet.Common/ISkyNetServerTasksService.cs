using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [ServiceContract]
    public interface ISkyNetServerTasksService
    {
        [OperationContract]
        [WebInvoke(Method = "CREATE", UriTemplate = "task", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string AddTask(ServerTask task);

        [OperationContract]
        [WebGet(UriTemplate = "task/all", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IList<ServerTask> GetTasks();

        [OperationContract]
        [WebInvoke(Method="POST", UriTemplate = "task/{id}/cancel", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void CancelTask(string id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "task/{id}/terminate", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void TerminateTask(string id);

        [OperationContract]
        [WebGet(UriTemplate = "task/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServerTask GetTask(string id);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "task/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string DeleteTask(string id);
    }
}
