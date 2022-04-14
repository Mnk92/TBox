using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mnk.Library.ScriptEngine;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.Library.WpfControls.Tools;

namespace TBox.Data.TeamManager
{
	public class SubversionScript : IReportScript
	{
        [String("yyyy-MM-dd")]
        public string DateTimeFormat { get; set; }
        [String(CanBeEmpty=true)]
        public string Login { get; set; }
        [Password(CanBeEmpty = true)]
        public string Password { get; set; }
        [String]
        public string RepositoryUrl { get; set; }
        [File("svn.exe")]
        public string Executable { get; set; }

        public IDictionary<string, string> Aliases { get; set; }

	    public void Run(IReportScriptContext context)
	    {
            context.Configure(false);
			var info = new ProcessStartInfo
			{
				FileName = "cmd.exe",
                Arguments = GetArgs(context.DateFrom, context.DateTo),
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};
			using (var p = Process.Start(info) )
			{
				var s = p.StandardOutput;
				while (!s.EndOfStream)
				{
					var line = s.ReadLine();
					if (!IsDivider(line)) continue;
					line = p.StandardOutput.ReadLine();
					if (string.IsNullOrEmpty(line)) continue;
					var cols = line.Split('|');
					if (cols.Length < 4) continue;
                    var email = GetEmail(cols[1].Trim(), context.Persons);
					var date = DateTime.ParseExact(cols[2].Trim().Split(' ').FirstOrDefault(), DateTimeFormat, new DateTimeFormatInfo());
					var lines = ParseInt(cols[3])+1;
					while (lines-- > 0)
					{
						line = s.ReadLine();
						if (string.IsNullOrEmpty(line) || !line.StartsWith("#")) continue;
                        if (!string.IsNullOrEmpty(email))
						{
							context.AddResult(new LoggedInfo
							{
								Email = email,
								Date = date,
								Task = ParseInt(line.Substring(1)).ToString(CultureInfo.InvariantCulture),
								Column = context.Name
							});
						}
					}
				}
			}
		}

        private string GetAlias(string name)
        {
            return Aliases.ContainsKey(name) ? Aliases[name] : name;
        }

	    private string GetEmail(string email, IList<string> emails)
	    {
	        email = GetAlias(email);
	        if (emails.Contains(email))
	            return email;
            return TryGetEmail(email, emails);
	    }

	    private string TryGetEmail(string email, IList<string> emails)
	    {
            return emails.FirstOrDefault(x =>
                (x.Split('@').FirstOrDefault() ?? string.Empty)
                .Replace("_", ".")
                .StartsWith(email, StringComparison.InvariantCultureIgnoreCase));
	    }

	    private static int ParseInt(string text)
		{
			if (string.IsNullOrEmpty(text)) return 0;
			text = text.Trim();
			var numbers = 0;
			while (numbers < (text.Length - 1) && char.IsDigit(text[numbers]))
			{
				++numbers;
			}
			return int.Parse(text.Substring(0, numbers));
		}

		private static bool IsDivider(string line)
		{
			return !string.IsNullOrEmpty(line) && line.Trim().All(x => x == '-');
		}

		private string GetArgs(DateTime dateFrom, DateTime dateTo)
		{
			var credentials = string.Empty;
			if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
			{
				credentials = string.Format("--username {0} --password {1}",
					Login, Password.DecryptPassword());
			}
            var args = string.Format("/c \"{4}\" log -r{{{0}}}:{{{1}}} {2} {3}",
                FormatDate(dateFrom), FormatDate(dateTo), credentials, RepositoryUrl, Executable);
			return args;
		}

		private string FormatDate(DateTime dateFrom)
		{
			return dateFrom.ToString(DateTimeFormat);
		}

	}
}
