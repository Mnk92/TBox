using System.IO;
using Common.Base;
using Common.Base.Log;
using Common.Console;

namespace ProjectMan.Code
{
	class MsBuildProvider : IMsBuildProvider
	{
		private readonly string rootDir;
		private static readonly ILog Log = LogManager.GetLogger<MsBuildProvider>();
		public string PathToMsBuild { get; private set; }

		public MsBuildProvider(string pathToMsBuild, string rootDir)
		{
			this.rootDir = rootDir;
			PathToMsBuild = pathToMsBuild;
		}

		public void Build(string mode, string path, bool waitEnd = false)
		{
			var cmd = Path.Combine(rootDir, "msbuild.cmd");
			if(!File.Exists(cmd))Log.Write("Can't find: {0}", cmd );
			Cmd.Start(cmd, Log,
				string.Format("\"{0}\" \"{1}\" \"{2}\"",
				PathToMsBuild, path, mode), waitEnd);
		}
	}
}
