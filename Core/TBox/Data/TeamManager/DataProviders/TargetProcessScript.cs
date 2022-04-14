using System;
using System.Globalization;
using System.Net;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using Mnk.Library.ScriptEngine;
using ServiceStack.Text;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.Library.WpfControls.Tools;

namespace TBox.Data.TeamManager
{
    public class TargetProcessScript : IReportScript
    {
        class AssignableItem
        {
            public int Id { get; set; }
        }
        class UserItem
        {
            public int Id { get; set; }
            public string Email { get; set; }
        }
        class TimeItem
        {
            public double Spent { get; set; }
            public DateTime Date { get; set; }
            public AssignableItem Assignable { get; set; }
            public UserItem User { get; set; }
        }

        class TimeReport
        {
            public TimeItem[] Items { get; set; }
        }

        [String("dd-MM-yyyy")]
        public string DateTimeFormat { get; set; }
        [String]
        public string Login { get; set; }
        [Password]
        public string Password { get; set; }
        [String("https://targetprocess.com/TargetProcess2/")]
        public string TargetProcessUrl { get; set; }
        [String("{0}/restui/tpview.aspx#task/{1}", CanBeEmpty = false)]
        public string LinkUrlTemplate { get; set; }

        public IDictionary<string, string> Aliases { get; set; }

        public void Run(IReportScriptContext context)
        {
            context.Configure(true);
            using (var cl = CreateClient())
            {
				foreach(var p in context.Persons)
				{
	                var query = string.Format(
                        "Times?include=[spent,user[email],date,assignable[id]]&format=json&take=1000000&where=(user.email%20in%20({2}))and(date%20gte'{0}')and(date%20lte'{1}')",
                        FormatDate(context.DateFrom), FormatDate(context.DateTo), string.Join(",", "'" + GetAlias(p) + "'"));
					foreach (var x in JsonSerializer.DeserializeFromString<TimeReport>(cl.DownloadString(BuildUrl(query))).Items)
					{
						var task = x.Assignable != null ? x.Assignable.Id.ToString(CultureInfo.InvariantCulture) : "DELETED";
						context.AddResult(new LoggedInfo
						{
							Date = x.Date,
							Email = p,
							Task = task,
							Link = string.Format(LinkUrlTemplate, TargetProcessUrl, task),
							Spent = x.Spent,
							Column = context.Name
						});
					}
				}
            }
        }

        private string GetName(string name)
        {
            foreach (var i in Aliases)
            {
                if (string.Equals(i.Value, name, StringComparison.InvariantCultureIgnoreCase)) return i.Key;
            }
            return name;
        }

        private string GetAlias(string name)
        {
            return Aliases.ContainsKey(name) ? Aliases[name] : name;
        }

        private string FormatDate(DateTime dateFrom)
        {
            return dateFrom.ToString(DateTimeFormat);
        }

        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 20 * 60 * 1000;
                return w;
            }
        }

        private WebClient CreateClient()
        {
            var cl = new MyWebClient { Credentials = new NetworkCredential(Login, GetPassword()) };
            cl.Headers.Add("Content-Type", "application/json");
            return cl;
        }

        private string GetPassword()
        {
            return Password.DecryptPassword();
        }

        private Uri BuildUrl(string relativeUri)
        {
            var url = TargetProcessUrl;
            if (!string.IsNullOrEmpty(url) && url[url.Length - 1] != '/')
            {
                url += "/";
            }
            return new Uri(new Uri(url + "api/v1/"), relativeUri);
        }
    }
}
