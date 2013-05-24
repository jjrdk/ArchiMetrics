using System;
using System.Collections.Generic;
using System.Linq;
using ArchiCop.Data;

namespace ArchiCop.Core
{
    public class GraphService
    {
        private readonly List<ArchiCopGraph> _graphs = new List<ArchiCopGraph>();

        public GraphService(IInfoRepository repository)
        {
            IEnumerable<GraphRow> graphData = repository.GetGraphData();
            foreach (string graphName in graphData.GroupBy(item => item.GraphName).Select(g => g.Key))
            {
                ArchiCopGraph info = GetGraphInfo(graphData.Where(item => item.GraphName == graphName));
                _graphs.Add(info);
            }

            IEnumerable<DataSourceRow> datasourcesData = repository.GetDataSourceData();
            foreach (string dataSource in datasourcesData.GroupBy(item => item.DataSourceName).Select(g => g.Key))
            {
                ArchiCopGraph info = GetGraphInfo(datasourcesData.First(item => item.DataSourceName == dataSource));
                _graphs.Add(info);
            }
        }

        public IEnumerable<ArchiCopGraph> Graphs
        {
            get { return _graphs; }
        }

        private ArchiCopGraph GetGraphInfo(DataSourceRow dataSource)
        {
            ArchiCopGraph graph = GetGraph(dataSource.LoadEngineType, dataSource.Arg1, dataSource.Arg2);

            graph.DisplayName = dataSource.DataSourceName;

            return graph;
        }

        private ArchiCopGraph GetGraphInfo(IEnumerable<GraphRow> data)
        {
            var graph = new ArchiCopGraph();

            if (data.FirstOrDefault(item => item.RuleType == "LoadEngine") != default(GraphRow))
            {
                string loadEngine = data.FirstOrDefault(item => item.RuleType == "LoadEngine").RuleValue;
                string arg1 = data.FirstOrDefault(item => item.RuleType == "LoadEngine").Arg1;
                string arg2 = data.FirstOrDefault(item => item.RuleType == "LoadEngine").Arg2;

                var vertexRegexRules = new List<VertexRegexRule>();
                foreach (GraphRow rule in data.Where(item => item.RuleType == "VertexRegexRule"))
                {
                    vertexRegexRules.Add(new VertexRegexRule {Pattern = rule.RulePattern, Value = rule.RuleValue});
                }

                graph = GetGraph(loadEngine, arg1, arg2, vertexRegexRules.ToArray());
            }

            if (data.FirstOrDefault(item => item.RuleType == "DisplayName") != default(GraphRow))
            {
                graph.DisplayName = data.FirstOrDefault(item => item.RuleType == "DisplayName").RuleValue;
            }

            return graph;
        }

        public ArchiCopGraph GetGraph(string loadEngine, string arg1, string arg2,
                                      params VertexRegexRule[] vertexRegexRules)
        {
            var graph = new ArchiCopGraph();

            Type loadEngineType = Type.GetType(loadEngine);

            if (loadEngineType != null)
            {
                IEnumerable<ArchiCopEdge> edges;

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