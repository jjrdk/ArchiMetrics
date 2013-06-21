using System.Collections.Generic;

namespace ArchiCop.Data
{
    public class GraphInfo
    {
        public GraphInfo()
        {
            Rules = new List<GraphRuleInfo>();
        }

        public string GraphName { get; set; }
        public string DisplayName { get; set; }

        public List<GraphRuleInfo> Rules { get; private set; }

        public LoadEngineInfo LoadEngine { get; set; }
    }
}