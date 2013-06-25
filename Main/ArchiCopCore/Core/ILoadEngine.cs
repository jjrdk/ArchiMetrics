namespace ArchiCop.Core
{
    public interface ILoadEngine
    {
        ArchiCopGraph<ArchiCopVertex> LoadGraph();
    }
}