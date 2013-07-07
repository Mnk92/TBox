using System.Runtime.Serialization;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [DataContract]
    public class ServerConfig
    {
        [DataMember]
        public int Port { get; set; }
        [DataMember]
        public int MaximumTaskExecutionTime { get; set; }

        public ServerConfig()
        {
            Port = 30000;
            MaximumTaskExecutionTime = 60;
        }
    }
}
