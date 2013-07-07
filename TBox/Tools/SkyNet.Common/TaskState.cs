using System.Runtime.Serialization;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [DataContract]
    public enum TaskState
    {
        Idle,
        InProgress,
        Done
    }
}
