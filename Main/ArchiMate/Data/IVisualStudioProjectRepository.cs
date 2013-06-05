using System.Collections.Generic;

namespace ArchiMate.Data
{
    public interface IVisualStudioProjectRepository
    {
        IEnumerable<VisualStudioProjectRoot> GetProjects(IEnumerable<string> fileNames);
        VisualStudioProjectRoot GetSingleProject(string fileName);
    }
}