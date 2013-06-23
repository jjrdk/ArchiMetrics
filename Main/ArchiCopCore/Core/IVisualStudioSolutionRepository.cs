using ArchiCop.VisualStudioProjectData;

namespace ArchiCop.Core
{
    public interface IVisualStudioSolutionRepository
    {
        void CreateNewSolution(VisualStudioProjectGraph graph, string solutionFileName);
    }
}