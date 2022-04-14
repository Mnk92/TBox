using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;

namespace Mnk.TBox.Core.PluginsShared.Tools
{
	public class CassiniRunner
	{
		private readonly ILog log = LogManager.GetLogger<CassiniRunner>();

		public void Run( string path, int port, string vpath, bool asAdmin, string pathToDevServer, string pathToBrowser, bool runBrowser)
		{
			if(!ExceptionsHelper.HandleException(
				() => RunServer(path, port, vpath, asAdmin, pathToDevServer),
				() => "Can't run dev server: " + pathToDevServer,
				log
				))
			{
				if (runBrowser)
				{
					ExceptionsHelper.HandleException(
					() => RunBrowser(port, vpath, pathToBrowser),
					() => "Can't run browser: " + pathToBrowser,
					log
					);
				}
			}
		}

		private static void RunBrowser(int port, string vpath, string path)
		{
			using (Process.Start(path, "http://localhost:" + port + vpath)){}
		}

		private void RunServer(string path, int port, string vpath, bool asAdmin, string pathToDevServer)
		{
			var args = string.Format(CultureInfo.InvariantCulture, "/path:\"{0}\" /port:{1} /vpath:{2}", path, port, vpath);
		    var pi = new ProcessStartInfo
		        {
		            Arguments = args,
		            FileName = pathToDevServer,
		            UseShellExecute = true,
		            CreateNoWindow = true
		        };
		    if (asAdmin) pi.Verb = "runas";
            try
            {
                using (Process.Start(pi)){}
            }
            catch (Exception)
            {
                pi.Verb = null;
                using (Process.Start(pi)) { }
            }
		}

		public void StopAll(string pathToDevServer)
		{
			ProcessProcesses(pathToDevServer, p=>p.Kill());
		}

		private void ProcessProcesses(string pathToDevServer, Action<Process> worker )
		{
			var name = Path.GetFileNameWithoutExtension(pathToDevServer);
			foreach (var p in Process.GetProcesses().Where(x => x.ProcessName.EqualsIgnoreCase(name)))
			{
				try
				{
					worker(p);
				}
				catch (Exception ex)
				{
					log.Write(ex, "Can't process: " + p.ProcessName);
					return;
				}
			}
		}
	}
}
