using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QuickGraph;

namespace ArchiCop.Core
{
    public class EdgeEngineRegex : IEdgeEngine
    {
        #region IEdgeEngine Members

        public IEnumerable<ArchiCopEdge> ConvertEdges(IEnumerable<ArchiCopEdge> edges,
                                                      IEnumerable<VertexRegexRule> rules)
        {
            var graph = new BidirectionalGraph<ArchiCopVertex, ArchiCopEdge>(false);

            foreach (ArchiCopEdge edge in edges)
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
                        graph.AddEdge(new ArchiCopEdge(sVertex, tVertex));
                    }
                }
            }

            foreach (ArchiCopVertex vertex in graph.Vertices)
            {
                vertex.InEdges = graph.InEdges(vertex).Count();
                vertex.OutEdges = graph.OutEdges(vertex).Count();

                vertex.OutDegree = graph.OutDegree(vertex);
                vertex.InDegree = graph.InDegree(vertex);
                vertex.Degree = graph.Degree(vertex);
            }

            return graph.Edges;
        }

        #endregion
    }
}