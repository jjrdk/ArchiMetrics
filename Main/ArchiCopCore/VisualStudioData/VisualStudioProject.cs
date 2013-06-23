using System.Collections.Generic;
using ArchiCop.Core;

namespace ArchiCop.VisualStudioData
{
    public class VisualStudioProject : ArchiCopVertex
    {
        public VisualStudioProject(string projectGuid, string projectName) : base(projectName)
        {
            ProjectGuid = projectGuid;
            ProjectName = projectName;

            ProjectReferences = new List<VisualStudioProjectProjectReference>();
            LibraryReferences = new List<VisualStudioProjectLibraryReference>();
        }


        public List<VisualStudioProjectProjectReference> ProjectReferences { get; private set; }

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