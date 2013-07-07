using System;
using System.IO;
using System.Linq;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.SaveLoad;
using Mnk.TBox.Plugins.Market.Interfaces.Contracts;

namespace Mnk.TBox.Plugins.Market.Service.Service
{
	class FileMan
	{
		private static readonly ILog Log = LogManager.GetLogger<FileMan>();
		public readonly string Folder;

		public FileMan(string subDirName)
		{
			Folder = Path.Combine(Shared.FolderName, subDirName);
		}

		private static void LogError(Exception ex, string author, string name, string type)
		{
			Log.Write(ex, "Error {2} file: '{0}\\{1}'", author, name, type);
		}

		private string GetPath(string author, string name)
		{
			return Path.Combine(Path.Combine(Folder, author), name);
		}

		public DataContract Read(string author, string name)
		{
			try
			{
				var dirPath = GetPath(author, name);
				if (Directory.Exists(dirPath))
				{
					var data = new DataContract();
					data.Length = ExtFile.LoadDirectoryFiles(
									Directory.GetFiles(dirPath), 
									out data.Descriptions, out data.FileByteStream);
					return data;
				}
			}
			catch (Exception ex)
			{
				LogError(ex, author, name, "download");
			}
			return null;
		}

		public bool Save(string author, string name, DataContract data)
		{
			try
			{
				var dir = GetPath(author, name);
				ExtFile.RecreateDirectory(dir);
				ExtFile.SaveDirectoryFiles(dir, data.Descriptions, data.FileByteStream);
				return true;
			}
			catch (Exception ex)
			{
				LogError(ex, author, name, "upload");
			}
			return false;
		}

		public void Delete(string author, string name)
		{
			var info = new DirectoryInfo(GetPath(author, name));
			if (info.Exists)
			{
				info.Delete(true);
			}
			if (info.Parent == null) return;
			if (info.Parent.GetDirectories().All(x => x.Attributes == FileAttributes.Hidden))
			{
				info.Parent.Delete(true);
			}
		}

	}
}
