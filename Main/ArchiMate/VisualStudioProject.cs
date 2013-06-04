using System;

namespace ArchiMate
{
    public class VisualStudioProject : Vertice
    {
        public VisualStudioProject(string id) : base(id)
        {
        }

        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }
        public Guid ProjectGuid { get; set; }
    }
}