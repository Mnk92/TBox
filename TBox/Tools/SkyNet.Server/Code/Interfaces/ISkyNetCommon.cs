using System.ServiceModel;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces
{
    [ServiceContract]
    public interface ISkyNetCommon : ISkyNetServerAgentsService, ISkyNetServerTasksService, ISkyNetFileService
    {
    }
}
