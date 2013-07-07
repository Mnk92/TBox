using System;
using System.IO;
using System.Threading;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls.Code.Log;

namespace Mnk.TBox.Core.PluginsShared.Watcher
{
	sealed class Watch : IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger<Watch>();
		private readonly ICaptionedLog log;
		private readonly FileSystemWatcher watcher;
		private readonly object lockObj = new object();
		private readonly FileAnalizer fileAnalizer;
		private readonly string workerId;

		public Watch(DirInfo dirInfo, ICaptionedLog log, IDataParser dataParser, string workerId)
		{
			this.log = log;
			this.workerId = workerId;
			fileAnalizer = new FileAnalizer(dirInfo, dataParser);
			watcher = new FileSystemWatcher(dirInfo.Path) { Filter = dirInfo.Mask };
			watcher.Changed += OnFileChange;
			watcher.Created += OnFileCreate;
			watcher.Deleted += OnFileDeleted;
			watcher.Renamed += OnFileRenamed;

			AddNewFiles(false);

			watcher.EnableRaisingEvents = true;
		}

		private void Add(FileInfo file)
		{
			ExceptionsHelper.HandleException(
				() => fileAnalizer.Add(file.Name, file.Length),
				() => workerId + ". Exception adding item.", 
				Log);
		}

		private void Del(string name)
		{
			ExceptionsHelper.HandleException(
				() => fileAnalizer.Remove(name),
				() => workerId + ". Exception remove item.", 
				Log);
		}

		private void OnFileRenamed(object sender, RenamedEventArgs e)
		{
			lock (lockObj)
			{
				Del(e.OldName);
				Add(new FileInfo(e.FullPath));
			}
		}

		private void OnFileDeleted(object sender, FileSystemEventArgs e)
		{
			lock (lockObj)
			{
				Del(e.Name);
			}
		}

		private void OnFileCreate(object sender, FileSystemEventArgs e)
		{
			lock (lockObj)
			{
				var fileInfo = new FileInfo(e.FullPath);
				Add(fileInfo);
				if (fileInfo.Length <= 2) return;
				fileAnalizer.Reset(e.Name);
				ExceptionsHelper.HandleException(
					() => ProcessFile(e.FullPath),
					() => workerId + ". Exception on file create.",
					Log);
			}
		}

		private void OnFileChange(object sender, FileSystemEventArgs e)
		{
			lock(lockObj)
			{
				ExceptionsHelper.HandleException(
					() => ProcessFile(e.FullPath),
					() => workerId + ". Exception on file change.",
					Log);
			}
		}

		private void AddNewFiles(bool checkCanRead)
		{
			ExceptionsHelper.HandleException(
                () => fileAnalizer.AddNewFiles(checkCanRead),
				() => workerId + ". Exception on add new files.",
				Log);
		}

		public void CheckFiles()
		{
			lock (lockObj)
			{
				DoCheckFiles();
			}
		}

		private void DoCheckFiles(int deep=3)
		{
			foreach (var fileName in fileAnalizer.GetFileNames())
			{
				try
				{
					var path = Path.Combine(watcher.Path, fileName);
					if (!File.Exists(path) || !ProcessFile(path))
					{
						fileAnalizer.Remove(fileName);
						if(deep>0)DoCheckFiles(deep-1);
						return;
					}
				}
				catch (Exception ex)
				{
					Log.Write(ex, workerId + ". Exception on check files!");
				}
			}
			AddNewFiles(true);
		}

		private bool ProcessFile(string path, int deep = 3)
		{
		    try
		    {
		        using (var s = new FileInfo(path).Open(FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write))
		        {
		            fileAnalizer.ProcessFile(Path.GetFileName(path), s, log);
		        }
		    }
		    catch (IOException)
		    {
		        Thread.Sleep(32);
		        return deep > 0 && ProcessFile(path, deep - 1);
		    }
            return true;
		}

		public void Dispose()
		{
			watcher.EnableRaisingEvents = false;
			watcher.Dispose();
		}
	}
}
