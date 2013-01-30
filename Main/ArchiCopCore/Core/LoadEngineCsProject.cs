using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using QuickGraph;

namespace ArchiCop.Core
{
    public class LoadEngineCsProject : LoadEngine
    {
        private static readonly XNamespace XNameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";
        private readonly List<string> _projects;

        public LoadEngineCsProject(string path)
        {
            _projects =
                new List<string>(Directory.GetFiles(path, "*csproj", SearchOption.AllDirectories));
        }

        protected override IEnumerable<ArchiCopEdge> GetEdges()
        {
            var edges = new List<ArchiCopEdge>();

            foreach (string project in _projects)
            {
                foreach (Reference reference in GetProjectDependencies(project))
                {
                    edges.Add(new ArchiCopEdge(new ArchiCopVertex(Path.GetFileNameWithoutExtension(project)),
                                               new ArchiCopVertex(reference.Name)));
                }
            }

            return edges;
        }

        private static IEnumerable<Reference> GetProjectDependencies(string path)
        {
            var list = new List<Reference>();

            XDocument document = XDocument.Load(path);

            IEnumerable<XElement> qProjectReferences = from e in document.Descendants(XNameSpace + "ProjectReference")
                                                       select e;
            foreach (XElement element in qProjectReferences)
            {
                var projectReference = new ProjectReference
                                           {
                                               Include = (string) element.Attribute("Include"),
                                               Project = element.Element(XNameSpace + "Project").Value,
                                               Name = element.Element(XNameSpace + "Name").Value
                                           };

                list.Add(projectReference);
            }

            IEnumerable<XElement> qReferences = from e in document.Descendants(XNameSpace + "Reference")
                                                select e;
            foreach (XElement element in qReferences)
            {
                var reference = new LibraryReference
                                    {
                                        Include = (string) element.Attribute("Include")
                                    };
                if (element.Element(XNameSpace + "SpecificVersion") != null)
                {
                    reference.SpecificVersion = element.Element(XNameSpace + "SpecificVersion").Value;
                }
                if (element.Element(XNameSpace + "RequiredTargetFramework") != null)
                {
                    reference.RequiredTargetFramework = element.Element(XNameSpace + "RequiredTargetFramework").Value;
                }
                if (element.Element(XNameSpace + "HintPath") != null)
                {
                    reference.HintPath = element.Element(XNameSpace + "HintPath").Value;
                }
                list.Add(reference);
            }


            return list;
        }

        #region Nested type: LibraryReference

        private class LibraryReference : Reference
        {
            public string Include { get; set; }
            public string SpecificVersion { get; set; }
            public string HintPath { get; set; }
            public string RequiredTargetFramework { get; set; }

            public override string Name
            {
                get { return Include.Split(',').Length > 1 ? Include.Split(',')[0] : Include; }
            }

            public string Version
            {
                get { return Include.Split(',').Length > 1 ? Include.Split(',')[1].Split('=')[1] : ""; }
            }
        }

        #endregion

        #region Nested type: ProjectReference

        private class ProjectReference : Reference
        {
            public string Include { get; set; }
            public string Project { get; set; }
        }

        #endregion

        #region Nested type: Reference

        private class Reference
        {
            public virtual string Name { get; set; }
        }

        #endregion
    }
}