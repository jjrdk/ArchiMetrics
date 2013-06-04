namespace ArchiMate
{
    public class Edge<TV>
        where TV : Vertice
    {
        public string Id { get; set; }
        public TV Source { get; set; }
        public TV Target { get; set; }
    }
}