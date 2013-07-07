namespace Mnk.TBox.Core.PluginsShared.Tools
{
	public interface ISvnProvider
	{
		void Do(string command, string path, bool waitEnd = false);
		void Do(string command, string path, string args, bool waitEnd = false);
		void Merge(string command, string path);
		string Path { get; }
	}
}
