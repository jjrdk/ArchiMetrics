using System.Collections.Generic;

namespace ArchiMate.Core
{
    public class Graph<T>
    {
        public Graph()
        {
            Vertices = new List<Vertex<T>>();
            Edges = new List<Edge<T>>();
        }

        public List<Vertex<T>> Vertices { get; private set; }
        public List<Edge<T>> Edges { get; private set; }

        public void MergeGraph(Graph<T> graph)
        {
            foreach (var edge in graph.Edges)
            {
                AddEdge(edge.Source, edge.Target);
            }
        }

        public void AddEdge(Vertex<T> source, Vertex<T> target)
        {
            if (!Vertices.Exists(item => item.Id == source.Id))
            {
                Vertices.Add(source);
            }
            if (!Vertices.Exists(item => item.Id == target.Id))
            {
                Vertices.Add(target);
            }

            if (!Edges.Exists(item => item.Id == source.Id + ";" + target.Id))
            {
                var edge = new Edge<T> {Id = source.Id + ";" + target.Id, Source = source, Target = target};
                Edges.Add(edge);
            }
        }
    }
}