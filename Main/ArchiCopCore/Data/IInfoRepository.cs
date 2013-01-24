using System.Collections.Generic;
using ArchiCop.Core;

namespace ArchiCop.Data
{
    public interface IInfoRepository
    {
        GraphInfo GetGraphInfoData(string excelsheetname);

        IEnumerable<string> GetGraphNames();
    }
}