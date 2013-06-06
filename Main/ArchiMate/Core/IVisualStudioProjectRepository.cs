using System.Collections.Generic;

namespace ArchiMate.Core
{
    public interface IVisualStudioProjectRepository
    {
        IEnumerable<VisualStudioProjectRoot> GetProjects(IEnumerable<string> fileNames);
        VisualStudioProjectRoot GetSingleProject(string fileName);
    }
}