using System.Collections.Generic;
using System.Linq;
using ArchiCop.Core;
using QuickGraph;

namespace ArchiCop.VisualStudioData
{
    public class VisualStudioProjectGraph : ArchiCopGraph<VisualStudioProject>
    {
        public VisualStudioProjectGraph(IEnumerable<Edge<VisualStudioProject>> edges)
            : base(edges)
        {

        }

        public VisualStudioProjectGraph(IEnumerable<VisualStudioProject> projects)
        {
            foreach (VisualStudioProject project in projects)
            {
                if (!ContainsVertex(project))
                {
                    AddVertex(project);
                }
            }

            foreach (VisualStudioProject projectFrom in projects)
            {
                foreach (VisualStudioProjectProjectReference projectReference in projectFrom.ProjectReferences)
                {
                    VisualStudioProject projectTo = projects.First(item => item.ProjectGuid == projectReference.Project);
                    AddEdge(new ArchiCopEdge<VisualStudioProject>(projectFrom, projectTo));
                }
            }
        }
    }
}