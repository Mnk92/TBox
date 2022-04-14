using System;

namespace Mnk.TBox.Core.PluginsShared.ReportsGenerator
{
    public class LoggedInfo
    {
        public string Email { get; set; }
        public double Spent { get; set; }
        public DateTime Date { get; set; }
        public string Task { get; set; }
        public string Link { get; set; }
        public bool Fake { get; set; }
        public string Column { get; set; }
        public bool HasTime { get; set; }


        public LoggedInfo()
        {
            Fake = false;
            HasTime = true;
        }

        public LoggedInfo Clone()
        {
            return new LoggedInfo
                {
                    Email = Email,
                    Spent = Spent,
                    Column = Column,
                    Date = Date,
                    Fake = Fake,
                    Link = Link,
                    Task = Task,
                    HasTime = HasTime
                };
        }
    }
}
