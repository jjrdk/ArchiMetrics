using System;

namespace ArchiMate
{
    public class VisualStudioProject : Vertice
    {
        public VisualStudioProject(string id) : base(id)
        {
        }

        public string ProjectName
        {
            get
            {
                return VertexName;
            }
        }

        public string ProjectPath { get; set; }

        public string ProjectGuid
        {
            get
            {
                return VertexName;
            }
        }
    }
}