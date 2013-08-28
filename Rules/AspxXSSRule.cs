using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class AspxXSSRule : IRule
    {
        private FileAnalyzer analyzer;

        public AspxXSSRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            if (analyzer.Filename.EndsWith(".aspx"))
            {

                IEnumerable<string> codes = Util.FindAllInlineCodes(this.analyzer.Raw);

                foreach (string code in codes)
                {
                    if (!code.Contains("Encode"))
                    {
                        if (!code.Contains("=="))
                        {
                            if (!code.Contains("!="))
                            {
                                if (code.Contains("Request"))
                                {
                                    retval.Add(new GenericVulnerability(this.analyzer.Filename, "There appears to be an XSS vulnerability.\n\n" + code, Color.Red, "XSS"));
                                }
                            }
                        }
                    }
                }

            }
            return retval;
        }
    }
}
