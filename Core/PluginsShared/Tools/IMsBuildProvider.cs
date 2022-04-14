namespace Mnk.TBox.Core.PluginsShared.Tools
{
    public interface IMsBuildProvider
    {
        void BuildSlnFile(string mode, string path, bool waitEnd = false);
        void BuildBuildFile(string path, string args);
        string PathToMsBuild { get; }
    }
}
