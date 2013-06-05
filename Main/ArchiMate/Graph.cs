using System.Collections.Generic;

namespace ArchiMate
{
    public class Graph<TV, TE>
        where TV : Vertice
        where TE : Edge<TV>, new()
    {
        public Graph()
        {
            Vertices = new List<TV>();
            Edges = new List<TE>();
        }

        public List<TV> Vertices { get; private set; }
        public List<TE> Edges { get; private set; }

        public void MergeGraph(Graph<TV, TE> graph)
        {
            foreach (TE edge in graph.Edges)
            {
                AddEdge(edge.Source, edge.Target);
            }
        }

        public void AddEdge(TV source, TV target)
        {
            if (!Vertices.Exists(item => item.VertexId == source.VertexId))
            {
                Vertices.Add(source);
            }
            if (!Vertices.Exists(item => item.VertexId == target.VertexId))
            {
                Vertices.Add(target);
            }

            if (!Edges.Exists(item => item.Id == source.VertexId + ";" + target.VertexId))
            {
                var edge = new TE { Id = source.VertexId + ";" + target.VertexId, Source = source, Target = target };
                Edges.Add(edge);
            }
        }
    }
}