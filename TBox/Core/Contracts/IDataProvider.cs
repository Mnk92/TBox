namespace Mnk.TBox.Core.Contracts
{
	public interface IDataProvider
	{
		string ReadOnlyDataPath { get; }
		string WritebleDataPath { get; }
		string ToolsPath { get; }
	}
}
