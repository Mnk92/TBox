using System;
using System.IO;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Plugins.TeamManager.Code.Settings
{
    public class Report 
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool FilterResultsByErrors { get; set; }
        public bool AddNotPresentDays { get; set; }
        public int WorkingHoursPerDay { get; set; }
        public string Style { get; set; }
        public string DayStatusStrategy { get; set; }
        public string ExcelFilePath { get; set; }
        public bool AutoGenerate { get; set; }
        public CheckableDataCollection<CheckableData> Links { get; set; }


        public Report()
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
            FilterResultsByErrors = true;
            AddNotPresentDays = true;
            WorkingHoursPerDay = 8;
            Style = "default.css";
            DayStatusStrategy = "Default.cs";
            ExcelFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "default.xlsx");
            AutoGenerate = true;
            Links = new CheckableDataCollection<CheckableData>();
        }

        public Report Clone()
        {
            return new Report
                {
                    DateFrom = DateFrom,
                    DateTo = DateTo,
                    FilterResultsByErrors = FilterResultsByErrors,
                    AddNotPresentDays = AddNotPresentDays,
                    WorkingHoursPerDay = WorkingHoursPerDay,
                    Style = Style,
                    DayStatusStrategy = DayStatusStrategy,
                    ExcelFilePath = ExcelFilePath,
                    AutoGenerate = AutoGenerate,
                    Links = Links.Clone(),
                };
        }
    }
}
