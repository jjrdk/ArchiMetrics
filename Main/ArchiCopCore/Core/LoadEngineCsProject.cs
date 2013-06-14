using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArchiCop.Core
{
    public class LoadEngineCsProject : LoadEngine
    {
        private readonly IEnumerable<string> _projects;

        public LoadEngineCsProject(string path)
        {
            _projects =
                new List<string>(Directory.GetFiles(path, "*csproj", SearchOption.AllDirectories));
            _projects = _projects.Select(Path.GetFullPath);
        }

        protected override IEnumerable<ArchiCopEdge> GetEdges()
        {
            var edges = new List<ArchiCopEdge>();

            foreach (string project in _projects)
            {
                foreach (string reference in GetProjectDependencies(project))
                {
                    edges.Add(new ArchiCopEdge(new ArchiCopVertex(Path.GetFileNameWithoutExtension(project)),
                                               new ArchiCopVertex(reference)));
                }
            }

            return edges;
        }

        private static IEnumerable<string> GetProjectDependencies(string path)
        {
            IVisualStudioProjectRepository repository = new VisualStudioProjectRepository();
            VisualStudioProjectRoot projectRoot = repository.GetSingleProject(path);

            var list = projectRoot.Projects.Select(project => project.ProjectName).ToList();
            list.AddRange(projectRoot.Libraries.Select(library => library.Name));
            
            return list;
        }
    }
}