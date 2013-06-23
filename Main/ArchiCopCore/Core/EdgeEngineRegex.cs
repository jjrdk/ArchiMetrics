using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QuickGraph;

namespace ArchiCop.Core
{
    public class EdgeEngineRegex : IEdgeEngine
    {
        #region IEdgeEngine Members

        public IEnumerable<ArchiCopEdge<ArchiCopVertex>> ConvertEdges(IEnumerable<ArchiCopEdge<ArchiCopVertex>> edges,
                                                                      IEnumerable<VertexRegexRule> rules)
        {
            var graph = new BidirectionalGraph<ArchiCopVertex, ArchiCopEdge<ArchiCopVertex>>(false);

            foreach (var edge in edges)
            {
                string source = string.Empty;
                string target = string.Empty;

                foreach (VertexRegexRule rule in rules)
                {
                    if (source == string.Empty)
                    {
                        if (Regex.IsMatch(edge.Source.Name, rule.Pattern))
                        {
                            source = rule.Value;
                        }
                    }

                    if (target == string.Empty)
                    {
                        if (Regex.IsMatch(edge.Target.Name, rule.Pattern))
                        {
                            target = rule.Value;
                        }
                    }
                }

                if (source == string.Empty)
                {
                    source = edge.Source.Name;
                }
                if (target == string.Empty)
                {
                    target = edge.Target.Name;
                }

                ArchiCopVertex sVertex = graph.Vertices.FirstOrDefault(item => item.Name == source);
                if (sVertex == null)
                {
                    if (!string.IsNullOrEmpty(source))
                    {
                        sVertex = new ArchiCopVertex(source);
                        graph.AddVertex(sVertex);
                    }
                }

                ArchiCopVertex tVertex = graph.Vertices.FirstOrDefault(item => item.Name == target);
                if (tVertex == null)
                {
                    if (!string.IsNullOrEmpty(target))
                    {
                        tVertex = new ArchiCopVertex(target);
                        graph.AddVertex(tVertex);
                    }
                }

                if (sVertex != null & tVertex != null)
                {
                    if (!string.IsNullOrEmpty(sVertex.Name) & !string.IsNullOrEmpty(tVertex.Name))
                    {
                        graph.AddEdge(new ArchiCopEdge<ArchiCopVertex>(sVertex, tVertex));
                    }
                }
            }

            return graph.Edges;
        }

        #endregion
    }
}