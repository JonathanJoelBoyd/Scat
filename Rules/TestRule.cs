using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public class TestRule : IRule
    {
        private FileAnalyzer analyzer;

        public TestRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            retval.Add(new TestVulnerability(this.analyzer.Filename));

            return retval;
        }
    }
}
