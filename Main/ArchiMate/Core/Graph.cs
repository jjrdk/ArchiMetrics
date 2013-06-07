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
            foreach (Vertex<T> vertex in graph.Vertices)
            {
                AddVertex(vertex);
            }

            foreach (var edge in graph.Edges)
            {
                AddEdge(edge.Source, edge.Target);
            }
        }

        public Vertex<T> AddVertex(Vertex<T> vertex)
        {
            if (!Vertices.Exists(item => item.Id.ToLower() == vertex.Id.ToLower()))
            {
                Vertices.Add(vertex);
            }
            return vertex;
        }

        public Edge<T> AddEdge(Vertex<T> source, Vertex<T> target)
        {
            source = AddVertex(source);

            target = AddVertex(target);
            
            if (!Edges.Exists(item => item.Id.ToLower() == source.Id.ToLower() + ";" + target.Id.ToLower()))
            {
                var edge = new Edge<T> {Id = source.Id + ";" + target.Id, Source = source, Target = target};
                Edges.Add(edge);
                return edge;
            }

            return Edges.Find(item => item.Id.ToLower() == source.Id.ToLower() + ";" + target.Id.ToLower());             
        }
    }
}