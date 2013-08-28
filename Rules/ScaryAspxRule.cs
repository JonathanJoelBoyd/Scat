using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class ScaryAspxRule : IRule
    {
        private FileAnalyzer analyzer;

        public ScaryAspxRule(FileAnalyzer a)
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
                    // retval.Add(new GenericVulnerability(this.analyzer.Filename, "There appears to be an XSS vulnerability.\n\n" + code, Color.Red, "XSS"));


                    if (code.Contains("Request.QueryString") || code.Contains("Request.Form") || code.Contains("Request.Cook") || code.Contains("Request["))
                    {
                        foreach (string scaryMethod in Util.ScaryMethodNames)
                        {
                            if (code.Contains(scaryMethod))
                            {
                                string message = string.Format("There appears to be a scary method: {0} that can potentially be used by an attacker in the following code {1}", scaryMethod, code);
                                retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Red, "User-Directed Scary Method"));
                            }
                        }
                    }


                }

            }
            return retval;
        }
    }
}
