using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ArchiCop.Core;

namespace ArchiCop.VisualStudioProjectData
{
    public class BaseTest
    {
        protected static IEnumerable<VisualStudioProject> GetSampleProjects()
        {
            IVisualStudioProjectRepository projectRepository = new VisualStudioProjectRepository();
            string rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var projects = new List<string>(Directory.GetFiles(rootDirectory, "*.csproj", SearchOption.AllDirectories));

            return projectRepository.GetProjects(projects);
        }

        protected static VisualStudioProject GetSampleProject()
        {
            return GetSampleProjects().First(item => item.ProjectName == "ArchiCop");
        }
    }
}