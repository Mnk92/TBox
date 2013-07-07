using System;
using System.ServiceModel;
using Mnk.Library.Common.Log;

namespace Mnk.TBox.Plugins.Market.Service
{
	class Program
	{
		private static readonly ILog Log = LogManager.GetLogger<Program>();
		static void Main(string[] args)
		{
			LogManager.Init( new MultiplyLog( new IBaseLog[] { new FileLog( "log.log" ), new ConsoleLog() } ) );
			Console.WriteLine("***** Console Based WCF Host *****");
			try
			{
				using (var host = new ServiceHost(typeof(MarketService)))
				{
					host.Open();
					DisplayHostInfo(host);
					Console.WriteLine("The service is ready.");
					Console.WriteLine("Press the Enter key to terminate service.");
					Console.ReadLine();
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex, "Service is down.");
				Console.ReadLine();
			}
		}

		static void DisplayHostInfo(ServiceHost host)
		{
			Console.WriteLine();
			Console.WriteLine("***** Host Info *****");
			Console.WriteLine("Name: {0}", host.Description.ConfigurationName);
			foreach (var adress in host.BaseAddresses)
			{
				Console.WriteLine("Port: {0}", adress.Port);
				Console.WriteLine("LocalPath: {0}", adress.LocalPath);
				Console.WriteLine("Uri: {0}", adress.AbsoluteUri);
				Console.WriteLine("Scheme: {0}", adress.Scheme);
			}
			Console.WriteLine("*********************");
			Console.WriteLine();
		}
	}
}
