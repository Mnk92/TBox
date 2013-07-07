using System;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.Library.ScriptEngine;

namespace Mnk.TBox.Plugins.TeamManager.Code.Settings
{
    [Serializable]
    public sealed class Profile : Data
    {
        public Email Email { get; set; }
        public Report Report { get; set; }
        public CheckableDataCollection<Person> Persons { get; set; }
        public CheckableDataCollection<SingleFileOperation> Operations { get; set; }

        public Profile()
        {
            Email = new Email();
            Report = new Report();
            Persons = new CheckableDataCollection<Person>();
            Operations = new CheckableDataCollection<SingleFileOperation>();
        }

        public override object Clone()
        {
            return new Profile
            {
                Email = Email.Clone(),
                Report = Report.Clone(),
                Persons = Persons.Clone(),
                Operations = Operations.Clone(),
            };
        }
    }
}
