using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ArchiCop.Core
{
    public class EdgeEngineRegex : List<ArchiCopEdge>, IEdgeEngine
    {
        public EdgeEngineRegex(IEnumerable<ArchiCopEdge> edges, IEnumerable<VertexRegexRule> rules)
        {
            var vertices = new List<ArchiCopVertex>();

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

                ArchiCopVertex sVertex = vertices.FirstOrDefault(item => item.Name == source);
                if (sVertex == null)
                {
                    if (!string.IsNullOrEmpty(source))
                    {
                        sVertex = new ArchiCopVertex(source);
                        vertices.Add(sVertex);
                    }
                }

                ArchiCopVertex tVertex = vertices.FirstOrDefault(item => item.Name == target);
                if (tVertex == null)
                {
                    if (!string.IsNullOrEmpty(target) )
                    {
                        tVertex = new ArchiCopVertex(target);
                        vertices.Add(tVertex);
                    }                    
                }

                if (sVertex != null & tVertex != null)
                {
                    if (!string.IsNullOrEmpty(sVertex.Name) & !string.IsNullOrEmpty(tVertex.Name))
                    {
                        Add(new ArchiCopEdge(sVertex, tVertex));
                    }
                }
            }
        }
    }
}