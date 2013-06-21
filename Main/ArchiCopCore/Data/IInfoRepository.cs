using System.Collections.Generic;

namespace ArchiCop.Data
{
    public interface IInfoRepository
    {
        IEnumerable<GraphInfo> Graphs();
        IEnumerable<DataSourceInfo> DataSources();
    }
}