using System.Collections.Generic;
using ArchiCop.Core;

namespace ArchiMate.Core
{
    public class VisualStudioProjectGraph : Graph<VisualStudioProject>
    {
        public VisualStudioProjectGraph(IEnumerable<VisualStudioProjectRoot> projects)
        {
            foreach (VisualStudioProjectRoot project in projects)
            {
                var mergeGraph = new VisualStudioProjectGraph();

                var source = new Vertex<VisualStudioProject>(project.ProjectGuid, project.ProjectName);
                source.Data = project;

                mergeGraph.AddVertex(source);

                foreach (VisualStudioProject proj in project.Projects)
                {
                    var target = new Vertex<VisualStudioProject>(proj.ProjectGuid, proj.ProjectName);
                    target.Data = proj;

                    mergeGraph.AddEdge(source, target);
                }

                MergeGraph(mergeGraph);
            }
        }

        public VisualStudioProjectGraph()
        {
        }
    }
}