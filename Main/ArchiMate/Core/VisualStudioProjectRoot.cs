using System.Collections.Generic;

namespace ArchiMate.Core
{
    public class VisualStudioProjectRoot : VisualStudioProject
    {
        public VisualStudioProjectRoot()
        {
            Projects = new List<VisualStudioProject>();
        }

        public List<VisualStudioProject> Projects { get; private set; }
    }
}