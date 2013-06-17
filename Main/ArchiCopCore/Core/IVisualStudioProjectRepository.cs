using System.Collections.Generic;

namespace ArchiCop.Core
{
    public interface IVisualStudioProjectRepository
    {
        IEnumerable<VisualStudioProject> GetProjects(IEnumerable<string> fileNames);
    }
}