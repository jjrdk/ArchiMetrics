using System.Collections.Generic;

namespace ArchiCop.VisualStudioData
{
    public interface IVisualStudioProjectRepository
    {
        IEnumerable<VisualStudioProject> GetProjects(IEnumerable<string> fileNames);
    }
}