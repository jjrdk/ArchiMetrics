using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiCop
{
    public class GraphEngine : ArchiCopGraph
    {
        public GraphEngine(GraphInfo info)            
        {
            Type loadEngineType = Type.GetType(info.LoadEngine);

            if (loadEngineType != null)
            {
                IEnumerable<ArchiCopEdge> edges;

                if (info.Arg1 != null & info.Arg2 != null)
                {
                    edges =
                        (IEnumerable<ArchiCopEdge>)
                        Activator.CreateInstance(loadEngineType, new object[] { info.Arg1, info.Arg2 });
                }
                else if (info.Arg1 != null)
                {
                    edges =
                        (IEnumerable<ArchiCopEdge>)Activator.CreateInstance(loadEngineType, new object[] { info.Arg1 });
                }
                else
                {
                    edges = (IEnumerable<ArchiCopEdge>)Activator.CreateInstance(loadEngineType);
                }

                if (info.VertexRegexRules.Any())
                {
                    edges = new EdgeEngineRegex(edges, info.VertexRegexRules);
                }

                AddVerticesAndEdgeRange(edges);                
            }
        }
     
    }
}