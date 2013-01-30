using System.Collections.Generic;

namespace ArchiCop.Data
{
    public interface IInfoRepository
    {
        IEnumerable<GraphRow> GetGraphData();
        IEnumerable<DataSourceRow> GetDataSourceData();
    }
}