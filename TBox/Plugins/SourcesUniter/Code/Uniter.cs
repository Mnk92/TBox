using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnk.Library.Common.MT;
using Mnk.TBox.Locales.Localization.Plugins.SourcesUniter;

namespace Mnk.TBox.Plugins.SourcesUniter.Code
{
	sealed class Uniter
	{
		private static int lastTime = 0;
		private static void Update(Func<string> caption, Func<float> value, IUpdater updater)
		{
			var time = Environment.TickCount;
			if (time - lastTime <= 200) return;
			updater.Update(caption(), value());
			lastTime = time;
		}

		private static IEnumerable<string> GetFiles(string path, string mask, int masksCount, int maskNo, IUpdater updater)
		{
			if (updater.UserPressClose) return new string[] { };
            Update(() => (SourcesUniterLang.SearchingForMask + mask), () => 0.5f * maskNo / (float)masksCount, updater);
			return Directory.GetFiles(path, mask, SearchOption.AllDirectories);
		}

		private static IEnumerable<string> GetFiles(string path, IList<string> masks, IUpdater updater)
		{
			var masksCount = masks.Count();
			return masks.SelectMany((mask, id) => GetFiles(path, mask, masksCount, id, updater));
		}

		private static string GetFileData(string path, bool removeEmptyLines)
		{
			if (removeEmptyLines)
			{
				return string.Join(Environment.NewLine,
								File.ReadAllLines(path).Where(x => !string.IsNullOrWhiteSpace(x)));
			}
			return File.ReadAllText(path);
		}

		private readonly object locker = new object();

		private void ProcessFile(string fileName, bool removeEmptyLines, IDictionary<string, string> output, IUpdater updater, int filesCount)
		{
			if (updater.UserPressClose) return;
			var result = new StringBuilder(fileName)
				.AppendLine().AppendLine(GetFileData(fileName, removeEmptyLines)).ToString();
			lock (locker)
			{
				output[fileName] = result;
                Update(() => (SourcesUniterLang.ProcessFile + Path.GetFileName(fileName)), () => (0.5f + 0.5f * output.Count / (float)filesCount), updater);
			}
		}

		public string ProcessFiles(IUpdater updater, string targetPath, IEnumerable<string> masks, bool removeEmptyLines)
		{
			var result = new Dictionary<string, string>();
			var files = GetFiles(targetPath, masks.ToArray(), updater).ToList();
			var filesCount = files.Count();
			Parallel.ForEach(files, file => ProcessFile(file, removeEmptyLines, result, updater, filesCount));
			return string.Join(Environment.NewLine, result.Select(x => x.Value));
		}
	}
}
