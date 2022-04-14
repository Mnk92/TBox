using System.Text.RegularExpressions;

namespace Mnk.TBox.Plugins.RequestsWatcher.Code.Parser
{
	class Constants
	{
		public readonly static Regex RegCreated =
			new Regex(@"System.Net.Sockets Information: \d{1,} : \[(?<pid>\d{1,})\] Socket#(?<socket>\d{1,}) - Created connection from (.{1,}) to (?<destination>.{1,})\.", RegexOptions.Compiled | RegexOptions.Singleline);
		public readonly static Regex RegData =
			new Regex(@"System.Net.Sockets Verbose: \d{1,} : \[(?<pid>\d{1,})\] (.{1,}) : (?<text>.{1,47}) : .{1,}", RegexOptions.Compiled | RegexOptions.Singleline);

		public readonly static Regex RegSendBegin = 
			new Regex(@"System.Net.Sockets Verbose: \d{1,} : \[(?<pid>\d{1,})\] Data from Socket#(?<socket>\d{1,})::Send", RegexOptions.Compiled | RegexOptions.Singleline);
		public readonly static Regex RegSendEnd =
			new Regex(@"System.Net.Sockets Verbose: \d{1,} : \[(?<pid>\d{1,})\] [Existing]{0,} Socket#(?<socket>\d{1,})::Send\(\)", RegexOptions.Compiled | RegexOptions.Singleline);

		public readonly static Regex RegReceiveBegin = 
			new Regex(@"System.Net.Sockets Verbose: \d{1,} : \[(?<pid>\d{1,})\] Data from Socket#(?<socket>\d{1,})::Receive", RegexOptions.Compiled | RegexOptions.Singleline);
		public readonly static Regex RegReceiveEnd =
			new Regex(@"System.Net.Sockets Verbose: \d{1,} : \[(?<pid>\d{1,})\] [Existing]{0,} Socket#(?<socket>\d{1,})::Receive\(\)", RegexOptions.Compiled | RegexOptions.Singleline);

		public readonly static Regex RegDispose = 
			new Regex(@"System.Net.Sockets Verbose: \d{1,} : \[(?<pid>\d{1,})\] Socket#(?<socket>\d{1,})::Dispose\(\)", RegexOptions.Compiled | RegexOptions.Singleline);

		public const string Pid = "pid";
		public const string Socket = "socket";
		public const string Text = "text";
		public const string Destination = "destination";

	}
}
