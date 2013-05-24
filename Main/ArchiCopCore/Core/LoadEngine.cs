using System.Collections.Generic;
using System.Linq;

namespace ArchiCop.Core
{
    public abstract class LoadEngine : ILoadEngine
    {
        private readonly ArchiCopGraph _graph = new ArchiCopGraph();

        public IEnumerable<ArchiCopEdge> LoadEdges()
        {
            foreach (ArchiCopEdge edge in GetEdges())
            {
                string source = edge.Source.Name;
                string target = edge.Target.Name;

                if (source != null & target != null)
                {
                    ArchiCopVertex sVertex = _graph.Vertices.FirstOrDefault(item => item.Name == source);
                    if (sVertex == null)
                    {
                        sVertex = new ArchiCopVertex(source);
                        _graph.AddVertex(sVertex);
                    }

                    ArchiCopVertex tVertex = _graph.Vertices.FirstOrDefault(item => item.Name == target);
                    if (tVertex == null)
                    {
                        tVertex = new ArchiCopVertex(target);
                        _graph.AddVertex(tVertex);
                    }

                    _graph.AddEdge(new ArchiCopEdge(sVertex, tVertex));
                }
            }

            return _graph.Edges;
        }

        protected abstract IEnumerable<ArchiCopEdge> GetEdges();
    }
}