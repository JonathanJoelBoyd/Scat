using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class WeakHashRule : IRule
    {
        private FileAnalyzer analyzer;

        public WeakHashRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            string raw = this.analyzer.Raw;

            if (raw.Contains("System.Security.Cryptography"))
            {
                if (  raw.Contains("MD5") )
                {
                    if (raw.Contains("ComputeHash("))
                    {
                        retval.Add(new GenericVulnerability(this.analyzer.Filename, "MD5 is considered weak crypto.  Consider using somethingelse.", Color.Yellow, "Weak Crypto"));
                    }
                }
            }
         
            return retval;
        }
    }
}
