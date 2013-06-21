using System;
using System.Collections.Generic;
using System.Linq;
using ArchiCop.Data;

namespace ArchiCop.Core
{
    public class GraphService
    {
        private readonly List<ArchiCopGraph<ArchiCopVertex>> _dataSources = new List<ArchiCopGraph<ArchiCopVertex>>();
        private readonly List<ArchiCopGraph<ArchiCopVertex>> _graphs = new List<ArchiCopGraph<ArchiCopVertex>>();

        public GraphService(IInfoRepository repository)
        {
            IEnumerable<GraphInfo> graphData = repository.Graphs();
            foreach (string graphName in graphData.GroupBy(item => item.Name).Select(g => g.Key))
            {
                ArchiCopGraph<ArchiCopVertex> info = GetGraphInfo(graphData.First(item => item.Name == graphName));
                _graphs.Add(info);
            }

            IEnumerable<DataSourceInfo> datasourcesData = repository.DataSources();
            foreach (string dataSource in datasourcesData.GroupBy(item => item.Name).Select(g => g.Key))
            {
                ArchiCopGraph<ArchiCopVertex> info =
                    GetDataSourceInfo(datasourcesData.First(item => item.Name == dataSource));
                _dataSources.Add(info);
            }
        }

        public IEnumerable<ArchiCopGraph<ArchiCopVertex>> DataSources
        {
            get { return _dataSources; }
        }

        public IEnumerable<ArchiCopGraph<ArchiCopVertex>> Graphs
        {
            get { return _graphs; }
        }

        private ArchiCopGraph<ArchiCopVertex> GetDataSourceInfo(DataSourceInfo dataSource)
        {
            ArchiCopGraph<ArchiCopVertex> graph = GetGraph(dataSource.LoadEngine);

            graph.DisplayName = dataSource.Name;

            return graph;
        }

        private ArchiCopGraph<ArchiCopVertex> GetGraphInfo(GraphInfo graphInfo)
        {
            IEnumerable<VertexRegexRule> rules = graphInfo.Rules.Select(
                item =>
                new VertexRegexRule
                    {
                        Pattern = item.RulePattern,
                        Value = item.RuleValue
                    });

            ArchiCopGraph<ArchiCopVertex> graph = GetGraph(graphInfo.LoadEngine, rules.ToArray());

            graph.DisplayName = graphInfo.DisplayName;

            return graph;
        }

        private ArchiCopGraph<ArchiCopVertex> GetGraph(LoadEngineInfo loadEngineInfo, params VertexRegexRule[] vertexRegexRules)
        {
            var graph = new ArchiCopGraph<ArchiCopVertex>();
            var loadEngine = (ILoadEngine) loadEngineInfo.CreateLoadEngine();
            IEnumerable<ArchiCopEdge<ArchiCopVertex>> edges = loadEngine.LoadEdges();

            if (vertexRegexRules.Any())
            {
                edges = new EdgeEngineRegex().ConvertEdges(edges, vertexRegexRules);
            }

            graph.AddVerticesAndEdgeRange(edges);
            
            return graph;
        }
    }
}