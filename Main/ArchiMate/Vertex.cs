namespace ArchiMate
{
    public class Vertex<T>
    {
        public Vertex(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }

        public T Data { get; set; }
    }
}