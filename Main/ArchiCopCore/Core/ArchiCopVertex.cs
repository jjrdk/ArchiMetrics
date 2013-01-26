namespace ArchiCop.Core
{
    public class ArchiCopVertex
    {
        public ArchiCopVertex(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public int OutEdges { get; set; }
        public int InEdges { get; set; }
        public int Degree { get; set; }
        public int InDegree { get; set; }
        public int OutDegree { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}