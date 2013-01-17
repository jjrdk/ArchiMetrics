using System.Collections.Generic;

namespace ArchiCop
{
    public interface IInfoRepository
    {
        GraphInfo GetGraphInfoData(string excelsheetname);

        IEnumerable<string> GetGraphNames();
    }
}