using System.Collections.Generic;
using ArchiCop.Core;

namespace ArchiCop.Data
{
    public interface IInfoRepository
    {
        GraphInfo GetGraphInfoData(string connectionString, string tableName);

        IEnumerable<string> GetGraphNames(string connectionString);
    }
}