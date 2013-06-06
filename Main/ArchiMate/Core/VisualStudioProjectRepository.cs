using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ArchiMate.Core
{
    public class VisualStudioProjectRepository : IVisualStudioProjectRepository
    {
        private static readonly XNamespace NameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public IEnumerable<VisualStudioProjectRoot> GetProjects(IEnumerable<string> fileNames)
        {
            return fileNames.Select(GetSingleProject).ToList();
        }

        public VisualStudioProjectRoot GetSingleProject(string fileName)
        {
            XDocument document = XDocument.Load(fileName);

            string projectName = Path.GetFileNameWithoutExtension(fileName);

            IEnumerable<XElement> qProjectGuid = from e in document.Descendants(NameSpace + "ProjectGuid")
                                                 select e;

            string projectGuid = qProjectGuid.First().Value;

            IEnumerable<XElement> qProjectReferences = from e in document.Descendants(NameSpace + "ProjectReference")
                                                       select e;

            var sourceProject = new VisualStudioProjectRoot
                {
                    ProjectGuid = projectGuid,
                    ProjectPath = fileName,
                    ProjectName = projectName,
                    ProjectType = Path.GetExtension(fileName)
                };

            foreach (XElement element in qProjectReferences)
            {
                var targetProject = new VisualStudioProject
                    {
                        ProjectGuid = element.Element(NameSpace + "Project").Value,
                        ProjectPath = (string)element.Attribute("Include"),
                        ProjectName = element.Element(NameSpace + "Name").Value                    
                    };

                string directoryname = Path.GetDirectoryName(fileName);

                targetProject.ProjectPath = Path.Combine(directoryname, targetProject.ProjectPath);
                targetProject.ProjectPath = Path.GetFullPath((new Uri(targetProject.ProjectPath)).LocalPath);

                targetProject.ProjectType = Path.GetExtension(fileName);

                sourceProject.Projects.Add(targetProject);
            }

            return sourceProject;
        }
    }
}