using System;
using System.IO;
using System.Reflection;
using Mnk.Library.Common.Log;

namespace Mnk.TBox.Tools.ConsoleScriptRunner
{
	class Loader
	{
		private readonly ILog log = LogManager.GetLogger<Loader>();
		private const string LibraryPath = "Libraries";

		public void Load(string rootPath)
		{
		    var dir = Path.Combine(rootPath, LibraryPath);
			if (Directory.Exists(dir))
			{
				System.Threading.Tasks.Parallel.ForEach(
					Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly),
					LoadAssembly
					);
			}
		}

		private void LoadAssembly(string filePath)
		{
			try
			{
				AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(filePath));
			}
			catch (Exception ex)
			{
				log.Write(ex, "Error loading library from: '{0}'", filePath);
			}
		}
	}
}
