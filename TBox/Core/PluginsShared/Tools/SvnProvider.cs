using Mnk.Library.Common;
using Mnk.Library.Common.Log;

namespace Mnk.TBox.Core.PluginsShared.Tools
{
	public class SvnProvider : ISvnProvider
	{
		private static readonly ILog Log = LogManager.GetLogger<SvnProvider>();
		public string Path { get; private set; }

		public SvnProvider(string pathToSvn)
		{
			Path = pathToSvn;
		}

		public void Do(string command, string path, bool waitEnd = false)
		{
			Do(command, path, "/closeonend:2", waitEnd);
		}

		public void Do(string command, string path, string args, bool waitEnd = false)
		{
			Cmd.Start(Path, Log,
				string.Format("/command:{0} {1} /path:\"{2}\" ", command, args, path),
				waitEnd, true);
		}

		public void Merge(string command, string path)
		{
			Cmd.Start(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "TortoiseMerge.exe"), Log,
				string.Format("/{0}:\"{1}\"", command, path), false, true);
		}
	}
}
