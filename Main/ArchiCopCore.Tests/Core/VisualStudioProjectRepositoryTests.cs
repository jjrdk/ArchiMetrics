using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiCop.Core
{
    [TestClass]
    public class VisualStudioProjectRepositoryTests
    {
        [TestMethod]
        public void ProjectHasCorrectLibraryReferences_NoNullProperties_Include()
        {
            //
           
            //            
            VisualStudioProject project = GetSampleProject();
            
            //
            //Serialize("projectlibraries.xml",project.Libraries);
            Assert.IsFalse(project.LibraryReferences.Any(item => string.IsNullOrEmpty(item.Include)));
        }

        [TestMethod]
        public void ProjectHasCorrectLibraryReferences_NoNullProperties_Name()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            //Serialize("projectlibraries.xml",project.Libraries);
            Assert.IsFalse(project.LibraryReferences.Any(item => string.IsNullOrEmpty(item.Name)));
        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_NoNullProperties_ProjectName()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            //Serialize("projectprojects.xml",project.Projects);
            Assert.IsFalse(project.Projects.Any(item => string.IsNullOrEmpty(item.ProjectName)));
        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_NoNullProperties_ProjectGuid()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsFalse(project.Projects.Any(item => string.IsNullOrEmpty(item.ProjectGuid)));
        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_NoNullProperties_ProjectPath()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsFalse(project.Projects.Any(item => string.IsNullOrEmpty(item.ProjectPath)));
        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_NoNullProperties_ProjectType()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsFalse(project.Projects.Any(item => string.IsNullOrEmpty(item.ProjectType)));
        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_ProjectName()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            var source = project.Projects.Select(item => item.ProjectName).ToArray();
            var target = new[] {"ArchiCopCore"};
            Assert.IsTrue(source.Intersect(target).Any());
            
        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_ProjectGuid()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            var source = project.Projects.Select(item => item.ProjectGuid).ToArray();
            var target = new[] { "3a4d7180-400e-486e-84b9-adf89ddb790f".ToLower() };
            Assert.IsTrue(source.Intersect(target).Any());

        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_ProjectPath()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //            
            var source = project.Projects.Select(item =>item.ProjectPath).ToArray();
            var target = new[] { @"c:\archicop - master\main\archicopcore.tests\bin\debug\core\archicopcore\archicopcore.csproj".ToLower() };
            
            Assert.IsTrue(source.Intersect(target).Any());

        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_ProjectType()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //            
            var source = project.Projects.Select(item => item.ProjectType).ToArray();
            var target = new[] { @".csproj" };
            Assert.IsTrue(source.Intersect(target).Any());

        }

        [TestMethod]
        public void ProjectHasCorrectNumberOfProjectReferences()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.Projects.Count == 1);
        }

        [TestMethod]
        public void ProjectHasCorrectNumberOfLibraryReferences()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.LibraryReferences.Count == 20);
        }

        [TestMethod]
        public void ProjectHasNoNullProperties()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            //Dictionary<string,string> propertiesWithValues = GetPropertiesWithValues(project);
            //foreach (KeyValuePair<string, string> keyValuePair in propertiesWithValues)
            //{
            //    Assert.IsFalse(string.IsNullOrEmpty(keyValuePair.Value),
            //                   string.Format("{0},{1}", keyValuePair.Key, keyValuePair.Value));
            //}
        }

        [TestMethod]
        public void ProjectHasCorrectProjectName()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.ProjectName == "ArchiCop");
        }

        [TestMethod]
        public void ProjectHasCorrectAssemblyName()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.AssemblyName == "aArchiCop");
        }

        [TestMethod]
        public void ProjectHasCorrectRootNamespace()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.RootNamespace == "nArchiCop");
        }

        [TestMethod]
        public void ProjectHasCorrectTargetFrameworkVersion()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.TargetFrameworkVersion == "v4.0");
        }

        [TestMethod]
        public void ProjectHasCorrectOutputType()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.OutputType == "WinExe");
        }

        [TestMethod]
        public void ProjectHasCorrectProjectGuid()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.ProjectGuid == "4A783824-257C-44B3-AF85-9A48CEB3B770".ToLower());
        }

        [TestMethod]
        public void ProjectHasCorrectProjectTypeGuids()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.ProjectTypeGuids == "60dc8134-eba5-43b8-bcc9-bb4bc16c2548;FAE04EC0-301F-11D3-BF4B-00C04F79EFBC".ToLower());
        }

        [TestMethod]
        public void ProjectHasCorrectProjectTypes()
        {
            //

            //            
            VisualStudioProject project = GetSampleProject();

            //
            Assert.IsTrue(project.ProjectTypes == "WPF;Windows (C#)");
        }

        private static VisualStudioProject GetSampleProject()
        {
            IVisualStudioProjectRepository projectRepository = new VisualStudioProjectRepository();
            string rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var projects = new List<string>(Directory.GetFiles(rootDirectory, "*.csproj", SearchOption.AllDirectories));

            return projectRepository.GetProjects(projects).First(item => item.ProjectName == "ArchiCop");
        }

        //private static IVisualStudioProjectRepository GetVisualStudioProjectRepository(out string projectFile)
        //{
        //    IVisualStudioProjectRepository projectRepository = new VisualStudioProjectRepository();
        //    string rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    projectFile = Path.Combine(rootDirectory, @"Core\ArchiCop.csproj");
        //    return projectRepository;
        //}

        //private Dictionary<string, string> GetPropertiesWithValues(object obj)
        //{
        //    return obj.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj, null) as string);
        //}

        //private void Serialize<T>(string filename,T obj)
        //{
        //    var serializer = new XmlSerializer(typeof(T));
        //    TextWriter writer = new StreamWriter(filename);

        //    serializer.Serialize(writer, obj);
        //    writer.Close();
        //}

        //private T Deserialize<T>(string filename)
        //{
        //    var serializer = new XmlSerializer(typeof(T));
        //    var fs = new FileStream(filename, FileMode.Open);
        //    var t = (T)serializer.Deserialize(fs);
        //    fs.Close();
        //    return t;
        //}
    }
}
