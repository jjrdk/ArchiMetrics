using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ArchiMate
{
    public class VisualStudioProjectGraph : Graph<VisualStudioProject, Edge<VisualStudioProject>>
    {
        private static readonly XNamespace NameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public VisualStudioProjectGraph(IEnumerable<string> projectFiles)
        {
            foreach (string projectsFileName in projectFiles)
            {
                var mergeGraph = new VisualStudioProjectGraph(projectsFileName);
                MergeGraph(mergeGraph);
            }
        }

        private VisualStudioProjectGraph(string filename)
        {           
            XDocument document = XDocument.Load(filename);

            string projectName = Path.GetFileNameWithoutExtension(filename);

            IEnumerable<XElement> qProjectGuid = from e in document.Descendants(NameSpace + "ProjectGuid")
                                                 select e;

            string projectGuid = qProjectGuid.First().Value;

            IEnumerable<XElement> qProjectReferences = from e in document.Descendants(NameSpace + "ProjectReference")
                                                       select e;

            var sourceProject = new VisualStudioProject(projectGuid)
                {
                    ProjectPath = filename,
                    ProjectGuid = new Guid(projectGuid),
                    ProjectName = projectName
                };

            foreach (XElement element in qProjectReferences)
            {
                var targetProject = new VisualStudioProject(element.Element(NameSpace + "Project").Value)
                    {
                        ProjectPath = (string) element.Attribute("Include"),
                        ProjectGuid = new Guid(element.Element(NameSpace + "Project").Value),
                        ProjectName = element.Element(NameSpace + "Name").Value
                    };

                string directoryname = Path.GetDirectoryName(filename);

                targetProject.ProjectPath = Path.Combine(directoryname, targetProject.ProjectPath);
                targetProject.ProjectPath = Path.GetFullPath((new Uri(targetProject.ProjectPath)).LocalPath);
                
                AddEdge(sourceProject, targetProject);
            }
        }
    }
}