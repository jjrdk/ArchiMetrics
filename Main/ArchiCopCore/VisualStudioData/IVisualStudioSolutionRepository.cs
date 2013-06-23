namespace ArchiCop.VisualStudioData
{
    public interface IVisualStudioSolutionRepository
    {
        void CreateNewSolution(VisualStudioProjectGraph graph, string solutionFileName);
    }
}