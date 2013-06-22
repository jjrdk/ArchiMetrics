using System.Collections.Generic;

namespace ArchiCop.Data
{
    public class ConfigInfo
    {
        public ConfigInfo(IEnumerable<DataSourceInfo> dataSources,IEnumerable<GraphInfo> graphs)
        {
            DataSources = dataSources;
            Graphs = graphs;
        }
        public IEnumerable<GraphInfo> Graphs { get; private set; }
        public IEnumerable<DataSourceInfo> DataSources { get; private set; }
    }

}