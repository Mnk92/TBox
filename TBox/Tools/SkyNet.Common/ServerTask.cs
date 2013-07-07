using System;
using System.Runtime.Serialization;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [DataContract]
    public class ServerTask
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Script { get; set; }

        [DataMember]
        public string Owner { get; set; }

        [DataMember]
        public string ScriptParameters { get; set; }

        [DataMember]
        public string ZipPackageId { get; set; }

        [DataMember]
        public string Report { get; set; }

        [DataMember]
        public int Progress { get; set; }

        [DataMember]
        public bool IsDone { get; set; }

        [DataMember]
        public TaskState State { get; set; }

        [DataMember]
        public DateTime CreatedTime { get; set; }

        public ServerTask()
        {
            CreatedTime = DateTime.UtcNow;
        }
    }
}
