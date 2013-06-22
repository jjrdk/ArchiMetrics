
using System.Collections.Generic;

namespace ArchiCop.Data
{
    public interface IInfoRepository
    {
        IEnumerable<ConfigInfo> GetConfigInfos(params string[] excelFileNames);
    }
}