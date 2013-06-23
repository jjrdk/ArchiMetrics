using System.Collections.Generic;

namespace ArchiCop.InfoData
{
    public interface IInfoRepository
    {
        IEnumerable<ConfigInfo> GetConfigInfos(params string[] excelFileNames);
    }
}