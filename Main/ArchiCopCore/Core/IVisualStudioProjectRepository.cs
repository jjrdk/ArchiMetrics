using System.Collections.Generic;

namespace ArchiCop.Core
{
    public interface IVisualStudioProjectRepository
    {
        IEnumerable<VisualStudioProjectRoot> GetProjects(IEnumerable<string> fileNames);
        VisualStudioProjectRoot GetSingleProject(string fileName);
    }
}