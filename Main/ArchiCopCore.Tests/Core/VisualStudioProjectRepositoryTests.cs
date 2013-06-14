using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiCop.Core
{
    [TestClass]
    public class VisualStudioProjectRepositoryTests
    {
        [TestMethod]
        public void ProjectHasCorrectProjectReferences_ProjectName()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            var source = project.Projects.Select(item => item.ProjectName).ToArray();
            var target = new[] {"ArchiCopCore"};
            Assert.IsTrue(source.Intersect(target).Any());
            
        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_ProjectGuid()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            var source = project.Projects.Select(item => item.ProjectGuid).ToArray();
            var target = new[] { "3a4d7180-400e-486e-84b9-adf89ddb790f" };
            Assert.IsTrue(source.Intersect(target).Any());

        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_ProjectPath()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //            
            var source = project.Projects.Select(item =>item.ProjectPath).ToArray();
            var target = new[] { @"C:\ArchiCop - master\Main\ArchiCopCore.Tests\bin\Debug\ArchiCopCore\ArchiCopCore.csproj".ToLower() };
            Assert.IsTrue(source.Intersect(target).Any());

        }

        [TestMethod]
        public void ProjectHasCorrectProjectReferences_ProjectType()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //            
            var source = project.Projects.Select(item => item.ProjectType).ToArray();
            var target = new[] { @".csproj" };
            Assert.IsTrue(source.Intersect(target).Any());

        }

        [TestMethod]
        public void ProjectHasCorrectNumberOfProjectReferences()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.Projects.Count == 1);
        }

        [TestMethod]
        public void ProjectHasCorrectNumberOfLibraryReferences()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.Libraries.Count == 20);
        }

        [TestMethod]
        public void ProjectHasCorrectProjectName()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.ProjectName == "ArchiCop");
        }

        [TestMethod]
        public void ProjectHasCorrectAssemblyName()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.AssemblyName == "aArchiCop");
        }

        [TestMethod]
        public void ProjectHasCorrectRootNamespace()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.RootNamespace == "nArchiCop");
        }

        [TestMethod]
        public void ProjectHasCorrectTargetFrameworkVersion()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.TargetFrameworkVersion == "v4.0");
        }

        [TestMethod]
        public void ProjectHasCorrectOutputType()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.OutputType == "WinExe");
        }

        [TestMethod]
        public void ProjectHasCorrectProjectGuid()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.ProjectGuid == "4A783824-257C-44B3-AF85-9A48CEB3B770");
        }

        [TestMethod]
        public void ProjectHasCorrectProjectTypeGuids()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.ProjectTypeGuids == "60dc8134-eba5-43b8-bcc9-bb4bc16c2548;FAE04EC0-301F-11D3-BF4B-00C04F79EFBC".ToUpper());
        }

        [TestMethod]
        public void ProjectHasCorrectProjectTypes()
        {
            //
            string projectFile;
            var projectRepository = GetVisualStudioProjectRepository(out projectFile);

            //            
            VisualStudioProjectRoot project = projectRepository.GetSingleProject(projectFile);

            //
            Assert.IsTrue(project.ProjectTypes == "WPF;Windows (C#)");
        }

        private static IVisualStudioProjectRepository GetVisualStudioProjectRepository(out string projectFile)
        {
            IVisualStudioProjectRepository projectRepository = new VisualStudioProjectRepository();
            string rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            projectFile = Path.Combine(rootDirectory, @"Core\ArchiCop.csproj");
            return projectRepository;
        }

    }
}
