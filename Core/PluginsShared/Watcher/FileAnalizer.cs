using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls.Code.Log;

namespace Mnk.TBox.Core.PluginsShared.Watcher
{
	class FileAnalizer
	{
	    private static readonly ILog Log = LogManager.GetLogger<FileAnalizer>();
		private readonly DirInfo dirInfo;
		private readonly IDataParser dataParser;
		private readonly Dictionary<string, long> files = new Dictionary<string, long>();

		public FileAnalizer(DirInfo dirInfo, IDataParser dataParser)
		{
			this.dirInfo = dirInfo;
			this.dataParser = dataParser;
		}

		public void Add(string name, long length)
		{
			files.Add(name, length);
		}

		public void Remove(string name)
		{
			files.Remove(name);
		}

		public void Reset(string name)
		{
			files[name]=0;
		}

		public void ProcessFile(string name, FileStream s, ICaptionedLog log)
		{
			var oldLength = files[name];
			var newLength = s.Length;
			if (oldLength >= newLength) return;
			files[name] = newLength;
			dataParser.Parse(dirInfo.Key, name, ReadData(s, oldLength, newLength), log);
		}

		public IList<string> GetFileNames()
		{
			return files.Keys.ToArray();
		}

		private string ReadData(Stream s, long oldLength, long newLength)
		{
			var data = new byte[newLength - oldLength];
			s.Seek(
				dirInfo.Direction == FileDirection.Down ? oldLength : 0, 
				SeekOrigin.Begin);
			s.Read(data, 0, data.Length);
			using (var sr = new StreamReader(s))
			{
				return sr.CurrentEncoding.GetString(data);
			}
		}

        public void AddNewFiles(bool checkCanRead)
		{
			var dir = new DirectoryInfo(dirInfo.Path);
			foreach (var file in dir.SafeEnumerateFiles(Log, dirInfo.Mask)
                .Where(x => !files.ContainsKey(x.Name)))
			{
                if (checkCanRead)
                {
                    try
                    {
                        using (file.Open(FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write))
                        {
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
				Add(file.Name, file.Length);
			}
		}
	}
}
