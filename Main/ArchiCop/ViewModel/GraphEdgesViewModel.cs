﻿using System.Collections.Generic;
using ArchiCop.Core;

namespace ArchiCop.ViewModel
{
    public class GraphEdgesViewModel : GraphViewModel
    {
        public GraphEdgesViewModel(ArchiCopGraph<ArchiCopVertex> graph, string displayName, string tag) :
            base(graph, displayName, tag)
        {
            EdgesToVisualize = graph.Edges;
            Sinks = graph.Sinks;
            Roots = graph.Roots;
            StronglyConnectedComponents = graph.StronglyConnectedComponents;
            TopologicalSort = graph.TopologicalSort;
            OddVertices = graph.OddVertices;
        }

        public IEnumerable<ArchiCopEdge<ArchiCopVertex>> EdgesToVisualize { get; private set; }
        public IEnumerable<ArchiCopVertex> Sinks { get; private set; }
        public IEnumerable<ArchiCopVertex> Roots { get; private set; }
        public IEnumerable<ArchiCopVertex> OddVertices { get; private set; }
        public IEnumerable<ArchiCopVertex> TopologicalSort { get; private set; }
        public IDictionary<ArchiCopVertex, int> StronglyConnectedComponents { get; private set; }
    }
}