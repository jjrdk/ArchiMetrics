using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph.Algorithms;

namespace ArchiCop.Core
{
    public class GraphEngine
    {
        private readonly ArchiCopGraph _graph = new ArchiCopGraph();
        private readonly IEnumerable<ArchiCopVertex> _oddVertices;
        private readonly IEnumerable<ArchiCopVertex> _roots;
        private readonly IEnumerable<ArchiCopVertex> _sinks;
        private readonly IDictionary<ArchiCopVertex, int> _stronglyConnectedComponents;
        private readonly IEnumerable<ArchiCopVertex> _topologicalSort;


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
                         Activator.CreateInstance(loadEngineType, new object[] {info.Arg1, info.Arg2})).LoadEdges();
                }
                else if (info.Arg1 != null)
                {
                    edges =
                        ((ILoadEngine) Activator.CreateInstance(loadEngineType, new object[] {info.Arg1})).LoadEdges();
                }
                else
                {
                    edges = ((ILoadEngine) Activator.CreateInstance(loadEngineType)).LoadEdges();
                }

                if (info.VertexRegexRules.Any())
                {
                    edges = new EdgeEngineRegex().ConvertEdges(edges, info.VertexRegexRules);
                }

                _graph.AddVerticesAndEdgeRange(edges);
            }

            _graph.StronglyConnectedComponents(out _stronglyConnectedComponents);
            _topologicalSort = _graph.TopologicalSort();
            _roots = _graph.Roots();
            _sinks = _graph.Sinks();
            _oddVertices = _graph.OddVertices();
        }

        public ArchiCopGraph Graph
        {
            get { return _graph; }
        }

        public IDictionary<ArchiCopVertex, int> StronglyConnectedComponents
        {
            get { return _stronglyConnectedComponents; }
        }

        public IEnumerable<ArchiCopVertex> TopologicalSort
        {
            get { return _topologicalSort; }
        }

        public IEnumerable<ArchiCopVertex> Roots
        {
            get { return _roots; }
        }

        public IEnumerable<ArchiCopVertex> Sinks
        {
            get { return _sinks; }
        }

        public IEnumerable<ArchiCopVertex> OddVertices
        {
            get { return _oddVertices; }
        }
    }
}