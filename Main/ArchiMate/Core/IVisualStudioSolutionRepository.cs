namespace ArchiMate.Core
{
    public interface IVisualStudioSolutionRepository
    {
        void CreateNewSolution(VisualStudioProjectGraph graph,string solutionFileName);
    }
}