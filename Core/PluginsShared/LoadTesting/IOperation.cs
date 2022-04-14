namespace Mnk.TBox.Core.PluginsShared.LoadTesting
{
	public interface IOperation
	{
		int Threads { get; set; }
		int Delay { get; set; }
		int Timeout { get; set; }
		bool IsChecked { get; set; }
		string Key { get; set; }
	}
}
