using ArchiCop.Core;

namespace ArchiCop.VisualStudioData
{
    public interface IVisualStudioSolutionRepository
    {
        void CreateNewSolution(ArchiCopGraph<VisualStudioProject> graph, string solutionFileName);
    }
}