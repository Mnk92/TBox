namespace ProjectMan.Code
{
	interface ISvnProvider
	{
		void Do(string command, string path, bool waitEnd = false);
		void Merge(string command, string path);
		string Path { get; }
	}
}
