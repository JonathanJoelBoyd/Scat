using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class DomBasedXssRule : IRule
    {
        private FileAnalyzer analyzer;

        public DomBasedXssRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            if (analyzer.Filename.EndsWith(".aspx"))
            {
                foreach (string line in analyzer.Lines)
                {
                    if (line.Contains("document.write") && line.Contains("document.location.href"))
                    {
                        if (!line.ToLower().Contains("encode"))
                        {
                            retval.Add(new GenericVulnerability(this.analyzer.Filename, "Potential Dom-Based XSS: " + line, Color.Orange, "Dom-based XSS"));
                        }
                    }

                    if (line.Contains("eval(") && line.Contains("document."))
                    {
                        if (!line.ToLower().Contains("encode"))
                        {
                            retval.Add(new GenericVulnerability(this.analyzer.Filename, "Potential Dom-Based XSS: " + line, Color.Orange, "Dom-based XSS"));
                        }
                    }    
                }
         

            }
            return retval;
        }
    }
}
