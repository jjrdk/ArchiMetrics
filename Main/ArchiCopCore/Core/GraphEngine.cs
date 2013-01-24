using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiCop.Core
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
                        ((ILoadEngine)
                        Activator.CreateInstance(loadEngineType, new object[] { info.Arg1, info.Arg2 })).LoadEdges();
                }
                else if (info.Arg1 != null)
                {
                    edges =
                        ((ILoadEngine) Activator.CreateInstance(loadEngineType, new object[] {info.Arg1})).LoadEdges();
                }
                else
                {
                    edges = ((ILoadEngine)Activator.CreateInstance(loadEngineType)).LoadEdges();
                }

                if (info.VertexRegexRules.Any())
                {
                    edges = new EdgeEngineRegex().ConvertEdges(edges, info.VertexRegexRules);
                }

                AddVerticesAndEdgeRange(edges);                
            }
        }
     
    }
}