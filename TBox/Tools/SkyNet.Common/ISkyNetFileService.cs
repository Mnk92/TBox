using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [ServiceContract]
    public interface ISkyNetFileService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "files", ResponseFormat = WebMessageFormat.Json)]
        string Upload(Stream stream);

        [OperationContract]
        [WebGet(UriTemplate = "files/{id}", RequestFormat = WebMessageFormat.Json)]
        Stream Download(string id);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "files/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Delete(string id);
    }
}
