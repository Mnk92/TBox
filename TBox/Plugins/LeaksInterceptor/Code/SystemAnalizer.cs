using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls.Components.Drawings.Graphics;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code
{
	sealed class SystemAnalizer : IDisposable
	{
		private readonly SafeTimer dataTimer;
		private Action onGetInfoFailed;
		private IList<IAnalizer> analizers;

		public bool Work { get { return dataTimer.Enabled; } }

		public SystemAnalizer()
		{
			dataTimer = new SafeTimer(DataTimerOnElapsed);
		}

		private void DataTimerOnElapsed()
		{
			if (analizers.ToArray().Any(analizer => !analizer.Analize()))
			{
				onGetInfoFailed();
			}
		}

		public void Start(int interval, Action onGetFailed, IList<IAnalizer> analizersInstances)
		{
			analizers = analizersInstances;
			onGetInfoFailed = onGetFailed;
			dataTimer.Start(interval);
		}

		public void Stop()
		{
			dataTimer.Stop();
			foreach (var d in analizers.ToArray().OfType<IDisposable>())
			{
				d.Dispose();
			}
		}

		public void CopyResultsToClipboard()
		{
			Clipboard.SetText(string.Join(Environment.NewLine, analizers.Select(x => x.CopyResults())));
		}

		public IGraphic GetGraphic(string name)
		{
			return analizers.ToArray()
				.Select(analizer => analizer.GetGraphic(name))
				.FirstOrDefault(g => g != null);
		}

		public void Dispose()
		{
			dataTimer.Dispose();
		}

		public IEnumerable GetNames()
		{
			return analizers.SelectMany(x => x.GetNames());
		}
	}
}
