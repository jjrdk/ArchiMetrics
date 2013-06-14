using System.Collections.Generic;

namespace ArchiCop.Core
{
    public class VisualStudioProject
    {
        public VisualStudioProject(string projectGuid, string projectName)
        {
            ProjectGuid = projectGuid;
            ProjectName = projectName;

            Projects = new List<VisualStudioProject>();

            ProjectReferences = new List<VisualStudioProjectProjectReference>();
            LibraryReferences = new List<VisualStudioProjectLibraryReference>();
        }

        public List<VisualStudioProject> Projects { get; private set; }

        internal List<VisualStudioProjectProjectReference> ProjectReferences { get; private set; }

        public List<VisualStudioProjectLibraryReference> LibraryReferences { get; private set; }

        public string ProjectName { get; private set; }

        public string ProjectPath { get; set; }

        public string ProjectGuid { get; private set; }

        public string ProjectType { get; set; }

        public string ProjectTypeGuids { get; set; }

        public string ProjectTypes { get; set; }

        public string TargetFrameworkVersion { get; set; }

        public string OutputType { get; set; }

        public string RootNamespace { get; set; }

        public string AssemblyName { get; set; }
    }
}