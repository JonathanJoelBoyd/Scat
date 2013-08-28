using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class HashWithoutSaltRule : IRule
    {
        private FileAnalyzer analyzer;

        public HashWithoutSaltRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            string raw = this.analyzer.Raw;

            if (raw.Contains("System.Security.Cryptography"))
            {
                if (raw.Contains("SHA1") || raw.Contains("SHA256") || raw.Contains("SHA384") || raw.Contains("SHA512") || raw.Contains("MD5"))
                {
                    if (raw.Contains("ComputeHash("))
                    {
                        if (!raw.ToLower().Contains("salt"))
                        {
                            retval.Add(new GenericVulnerability(this.analyzer.Filename, "Potential use of Hash without mention of Salt", Color.Yellow, "Weak Crypto"));
                        }
                    }
                }
            }
         
            return retval;
        }
    }
}
