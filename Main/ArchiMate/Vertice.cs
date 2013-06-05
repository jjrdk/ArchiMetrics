namespace ArchiMate
{
    public class Vertice
    {
        public Vertice(string id)
        {
            VertexId = id;
        }

        public string VertexId { get; private set; }
        public string VertexName { get; set; }
        public string VertexType { get; set; }
    }
}