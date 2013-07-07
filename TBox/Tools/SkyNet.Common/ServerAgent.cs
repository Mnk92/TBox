using System.Runtime.Serialization;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [DataContract]
    public class ServerAgent
    {
        [DataMember]
        public string Endpoint { get; set; }

        [DataMember]
        public int TotalCores { get; set; }

        [DataMember]
        public AgentState State { get; set; }
    }
}
