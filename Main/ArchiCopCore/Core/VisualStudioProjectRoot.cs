using System.Collections.Generic;

namespace ArchiCop.Core
{
    public class VisualStudioProjectRoot : VisualStudioProject
    {
        public VisualStudioProjectRoot(string projectGuid, string projectName)
            : base(projectGuid, projectName)
        {
            Projects = new List<VisualStudioProject>();
            Libraries = new List<VisualStudioProjectLibraryReference>();
        }

        public List<VisualStudioProject> Projects { get; private set; }

        public List<VisualStudioProjectLibraryReference> Libraries { get; private set; }
    }
}