namespace ArchiMate
{
    public class Edge<T>        
    {
        public string Id { get; set; }
        public Vertex<T> Source { get; set; }
        public Vertex<T> Target { get; set; }
    }
}