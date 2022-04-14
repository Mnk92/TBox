namespace Mnk.Rat
{
    public interface IIndexContextBuilder
    {
        void Rebuild();
        IndexContext Context { get; }
    }
}
