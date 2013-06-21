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
            foreach (string graphName in graphData.GroupBy(item => item.GraphName).Select(g => g.Key))
            {
                ArchiCopGraph<ArchiCopVertex> info = GetGraphInfo(graphData.First(item => item.GraphName == graphName));
                _graphs.Add(info);
            }

            IEnumerable<DataSourceInfo> datasourcesData = repository.DataSources();
            foreach (string dataSource in datasourcesData.GroupBy(item => item.DataSourceName).Select(g => g.Key))
            {
                ArchiCopGraph<ArchiCopVertex> info =
                    GetDataSourceInfo(datasourcesData.First(item => item.DataSourceName == dataSource));
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
            ArchiCopGraph<ArchiCopVertex> graph = GetGraph(dataSource.LoadEngine.EngineName, dataSource.LoadEngine.Arg1,
                                                           dataSource.LoadEngine.Arg2);

            graph.DisplayName = dataSource.DataSourceName;

            return graph;
        }

        private ArchiCopGraph<ArchiCopVertex> GetGraphInfo(GraphInfo graphInfo)
        {
            ArchiCopGraph<ArchiCopVertex> graph = GetGraph(graphInfo.LoadEngine.EngineName, graphInfo.LoadEngine.Arg1,
                                                           graphInfo.LoadEngine.Arg2,
                                                           graphInfo.Rules.Select(
                                                               item =>
                                                               new VertexRegexRule
                                                                   {
                                                                       Pattern = item.RulePattern,
                                                                       Value = item.RuleValue
                                                                   })
                                                                    .ToArray());

            graph.DisplayName = graphInfo.DisplayName;

            return graph;
        }

        private ArchiCopGraph<ArchiCopVertex> GetGraph(string loadEngine, string arg1, string arg2,
                                                       params VertexRegexRule[] vertexRegexRules)
        {
            var graph = new ArchiCopGraph<ArchiCopVertex>();

            Type loadEngineType = Type.GetType(loadEngine);

            if (loadEngineType != null)
            {
                IEnumerable<ArchiCopEdge<ArchiCopVertex>> edges;

                if (arg1 != null & arg2 != null)
                {
                    edges =
                        ((ILoadEngine)
                         Activator.CreateInstance(loadEngineType, new object[] {arg1, arg2})).LoadEdges();
                }
                else if (arg1 != null)
                {
                    edges =
                        ((ILoadEngine) Activator.CreateInstance(loadEngineType, new object[] {arg1})).LoadEdges();
                }
                else
                {
                    edges = ((ILoadEngine) Activator.CreateInstance(loadEngineType)).LoadEdges();
                }

                if (vertexRegexRules.Any())
                {
                    edges = new EdgeEngineRegex().ConvertEdges(edges, vertexRegexRules);
                }

                graph.AddVerticesAndEdgeRange(edges);
            }

            return graph;
        }
    }
}