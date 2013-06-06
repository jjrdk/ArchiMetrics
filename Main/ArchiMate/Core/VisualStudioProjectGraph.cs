using System.Collections.Generic;

namespace ArchiMate.Core
{
    public class VisualStudioProjectGraph : Graph<VisualStudioProject>
    {
        public VisualStudioProjectGraph(IEnumerable<VisualStudioProjectRoot> projects)
        {
            foreach (VisualStudioProjectRoot project in projects)
            {
                var mergeGraph = new VisualStudioProjectGraph();

                foreach (VisualStudioProject proj in project.Projects)
                {
                    var source = new Vertex<VisualStudioProject>(project.ProjectGuid, project.ProjectName);
                    source.Data = project;

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