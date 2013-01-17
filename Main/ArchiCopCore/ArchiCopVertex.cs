namespace ArchiCop
{
    public class ArchiCopVertex
    {
        public ArchiCopVertex(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}