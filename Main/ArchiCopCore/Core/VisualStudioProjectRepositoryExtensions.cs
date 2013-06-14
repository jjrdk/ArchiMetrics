using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ArchiCop.Core
{
    internal static class VisualStudioProjectRepositoryExtensions
    {
        private static readonly XNamespace NameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public static IEnumerable<VisualStudioProjectProjectReference> GetProjectReferences(this XDocument document, string fileName)
        {
            IEnumerable<XElement> references = from e in document.Descendants(NameSpace + "ProjectReference") select e;

            var list = new List<VisualStudioProjectProjectReference>();

            foreach (XElement reference in references)
            {
                var visualStudioProject =
                    new VisualStudioProjectProjectReference();

                visualStudioProject.Project = reference.Element(NameSpace + "Project").Value.TrimStart('{').TrimEnd('}').ToLower();
                visualStudioProject.Name=reference.Element(NameSpace + "Name").Value;
                visualStudioProject.Include = (string) reference.Attribute("Include");

                visualStudioProject.ProjectPath = visualStudioProject.Include;
                string directoryname = Path.GetDirectoryName(fileName);
                visualStudioProject.ProjectPath = Path.Combine(directoryname, visualStudioProject.ProjectPath);
                visualStudioProject.ProjectPath = Path.GetFullPath((new Uri(visualStudioProject.ProjectPath)).LocalPath).ToLower();

                visualStudioProject.ProjectType = Path.GetExtension(fileName).ToLower();

                list.Add(visualStudioProject);
            }

            return list;
        }

        public static IEnumerable<VisualStudioProjectLibraryReference> GetLibraryReferences(this XDocument document)
        {
            IEnumerable<XElement> references = from e in document.Descendants(NameSpace + "Reference") select e;

            var list = new List<VisualStudioProjectLibraryReference>();

            foreach (XElement reference in references)
            {
                var visualStudioProjectLibraryReference= new VisualStudioProjectLibraryReference();

                visualStudioProjectLibraryReference.Include = (string)reference.Attribute("Include");
                visualStudioProjectLibraryReference.HintPath = reference.GetPropertyGroupValue("HintPath");
                visualStudioProjectLibraryReference.SpecificVersion = reference.GetPropertyGroupValue("SpecificVersion");
                list.Add(visualStudioProjectLibraryReference);
            }

            return list;
        }

        public static string GetPropertyGroupValue(this XElement element, string elementName)
        {
            IEnumerable<XElement> query = from e in element.Descendants(NameSpace + elementName) select e;
            if (query.Any())
            {
                return query.First().Value;
            }
            return string.Empty;
        }
    }
}