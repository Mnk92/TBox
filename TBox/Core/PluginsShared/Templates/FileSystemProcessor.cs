using System.IO;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;

namespace Mnk.TBox.Core.PluginsShared.Templates
{
	class FileSystemProcessor : IFileSystemProcessor
	{
		private static readonly ILog Log = LogManager.GetLogger<FileSystemProcessor>();
		private readonly IStringFiller stringFiller;
		public FileSystemProcessor(IStringFiller stringFiller)
		{
			this.stringFiller = stringFiller;
		}

		public void Copy(string source, string destination)
		{
			var info = new DirectoryInfo(source);
			if (!Directory.Exists(destination))
				Directory.CreateDirectory(destination);
			foreach (var dir in info.SafeEnumerateDirectories(Log))
			{
				Copy(dir.FullName,
					Path.Combine(destination, stringFiller.Fill(dir.Name)));
			}
			foreach (var file in info.SafeEnumerateFiles(Log))
			{
				CopyFile(file.FullName,
						 Path.Combine(destination, stringFiller.Fill(file.Name)));
			}
		}

		private void CopyFile(string source, string destination)
		{
			File.WriteAllText(destination,
				stringFiller.Fill(File.ReadAllText(source)));
		}
	}
}
