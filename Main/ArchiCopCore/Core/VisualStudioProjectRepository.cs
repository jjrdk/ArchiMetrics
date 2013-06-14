using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ArchiCop.Core
{
    public class VisualStudioProjectRepository : IVisualStudioProjectRepository
    {
        private static readonly XNamespace NameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public IEnumerable<VisualStudioProject> GetProjects(IEnumerable<string> fileNames)
        {
            IEnumerable<VisualStudioProject> projects = fileNames.Select(GetSingleProject).ToList();

            foreach (VisualStudioProject visualStudioProject in projects)
            {
                foreach (VisualStudioProjectProjectReference projectReference in visualStudioProject.ProjectReferences)
                {
                    visualStudioProject.Projects.Add(projects.First(item=>item.ProjectGuid==projectReference.Project));
                }
            }

            return projects;
        }

        internal VisualStudioProject GetSingleProject(string fileName)
        {
            XDocument document = XDocument.Load(fileName);
            
            string projectName = Path.GetFileNameWithoutExtension(fileName);

            IEnumerable<XElement> qProjectGuid = from e in document.Descendants(NameSpace + "ProjectGuid")
                                                 select e;

            string projectGuid = qProjectGuid.First().Value.TrimStart('{').TrimEnd('}').ToLower();           

            var sourceProject = new VisualStudioProject(projectGuid, projectName)
                {
                    ProjectPath = fileName.ToLower(),
                    ProjectType = Path.GetExtension(fileName).ToLower()
                };

            sourceProject.LibraryReferences.AddRange(document.GetLibraryReferences());

            sourceProject.ProjectTypeGuids ="";
            foreach (string item in document.Root.GetPropertyGroupValue("ProjectTypeGuids").Split(';'))
            {
                sourceProject.ProjectTypeGuids = sourceProject.ProjectTypeGuids + item.TrimStart('{').TrimEnd('}') + ";";
            }

            sourceProject.ProjectTypeGuids = sourceProject.ProjectTypeGuids.ToLower().TrimEnd(';');

            string projectTypes = "";

            foreach (string item in sourceProject.ProjectTypeGuids.Split(';').Select(GetProjectType))
            {
                projectTypes = projectTypes + item + ";";
            }

            sourceProject.ProjectTypes = projectTypes.TrimEnd(';');


            sourceProject.TargetFrameworkVersion = document.Root.GetPropertyGroupValue("TargetFrameworkVersion");
            sourceProject.OutputType = document.Root.GetPropertyGroupValue("OutputType");
            sourceProject.RootNamespace = document.Root.GetPropertyGroupValue("RootNamespace");
            sourceProject.AssemblyName = document.Root.GetPropertyGroupValue("AssemblyName");

            sourceProject.ProjectReferences.AddRange(document.GetProjectReferences(fileName));

            return sourceProject;
        }

        public string GetProjectType(string projectTypeGuid)
        {
            /*
                Windows (C#)           {FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}
                Windows (VB.NET)       {F184B08F-C81C-45F6-A57F-5ABD9991F28F}
                Windows (Visual C++)   {8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}
                Web Application        {349C5851-65DF-11DA-9384-00065B846F21}
            Web Site               {E24C65DC-7377-472B-9ABA-BC803B73C61A}
            WCF                    {3D9AD99F-2412-4246-B90B-4EAA41C64699}
                WPF                    {60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}
            XNA (Windows)          {6D335F3A-9D43-41b4-9D22-F6F17C4BE596}
            XNA (XBox)             {2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2}
            XNA (Zune)             {D399B71A-8929-442a-A9AC-8BEC78BB2433}
                Silverlight            {A1591282-1198-4647-A2B1-27E5FF5F6F3B}
            ASP.NET MVC            {F85E285D-A4E0-4152-9332-AB1D724D3325}
            ASP.NET MVC 4          {E3E379DF-F4C6-4180-9B81-6769533ABE47}
                Test                   {3AC096D0-A1C2-E12C-1390-A8335801FDAB}
            Solution Folder        {2150E333-8FDC-42A3-9474-1A3956D46DE8}
            */

            switch (projectTypeGuid.ToUpper())
            {
                case "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC":
                    return "Windows (C#)";

                case "F184B08F-C81C-45F6-A57F-5ABD9991F28F":
                    return "Windows (VB.NET)";

                case "8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942":
                    return "Windows (Visual C++)";

                case "3AC096D0-A1C2-E12C-1390-A8335801FDAB":
                    return "Test";

                case "60DC8134-EBA5-43B8-BCC9-BB4BC16C2548":
                    return "WPF";

                case "A1591282-1198-4647-A2B1-27E5FF5F6F3B":
                    return "Silverlight";

                case "349C5851-65DF-11DA-9384-00065B846F21":
                    return "Web Application";

                default:
                    return projectTypeGuid;
            }
        }
    }
}