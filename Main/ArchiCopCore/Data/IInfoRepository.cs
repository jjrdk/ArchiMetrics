
using System.Collections.Generic;

namespace ArchiCop.Data
{
    public interface IInfoRepository
    {
        List<ConfigInfo> ConfigInfos { get; }
    }
}