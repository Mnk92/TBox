using Common.Base;
using Common.Base.Log;
using Common.Console;

namespace ProjectMan.Code
{
	class SvnProvider : ISvnProvider
	{
		private static readonly ILog Log = LogManager.GetLogger<SvnProvider>();
		public string Path { get; private set; }

		public SvnProvider(string pathToSvn)
		{
			Path = pathToSvn;
		}

		public void Do(string command, string path, bool waitEnd = false)
		{
			Cmd.Start(Path, Log,
				string.Format("/command:{0} /closeonend:3 /path:\"{1}\" ", command,path),
				waitEnd, true);
		}

		public void Merge(string command, string path)
		{
			Cmd.Start(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "TortoiseMerge.exe"), Log,
				string.Format("/{0}:\"{1}\"", command, path), false, true);
		}
	}
}
