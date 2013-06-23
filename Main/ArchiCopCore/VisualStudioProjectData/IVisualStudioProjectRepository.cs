using System.Collections.Generic;

namespace ArchiCop.VisualStudioProjectData
{
    public interface IVisualStudioProjectRepository
    {
        IEnumerable<VisualStudioProject> GetProjects(IEnumerable<string> fileNames);
    }
}