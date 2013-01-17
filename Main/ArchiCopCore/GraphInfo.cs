using System.Collections.Generic;

namespace ArchiCop
{
    public class GraphInfo
    {
        public string LoadEngine { get; set; }

        public string DisplayName { get; set; }

        public string Arg1 { get; set; }

        public string Arg2 { get; set; }

        public IEnumerable<RegexRule> EdgeRegexRules { get; set; }
    }
}