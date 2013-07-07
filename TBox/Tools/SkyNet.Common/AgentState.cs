using System.Runtime.Serialization;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [DataContract]
    public enum AgentState
    {
        Idle,
        InProgress,
        Died
    }
}
