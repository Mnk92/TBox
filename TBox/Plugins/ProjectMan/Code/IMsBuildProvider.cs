namespace ProjectMan.Code
{
	interface IMsBuildProvider
	{
		void Build(string mode, string path, bool waitEnd = false);
		string PathToMsBuild { get; }
	}
}
