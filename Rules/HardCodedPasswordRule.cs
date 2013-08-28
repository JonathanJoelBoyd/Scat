using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class HardCodedPasswordRule : IRule
    {
        private FileAnalyzer analyzer;

        public HardCodedPasswordRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            foreach (var c in this.analyzer.FileSyntaxAnalyzer.CodeClasses)
            {
                //
                // grab passwords from fields.
                //

                foreach (var f in c.CodeFields)
                {
                    if (f.Name.ToLower().Contains("secret") || f.Name.ToLower().Contains("password") || f.Name.ToLower().Contains("passwd"))
                    {
                        if (f.Code.Contains("=") && f.Code.Contains("\""))
                        {
                            string message = string.Format("Potential Hardcoded password: {0}", f.Code);
                            retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Orange, "Hardcoded Password"));
                        }
                    }
                }

                //
                // grab passwords from variable declaration statements
                //

                foreach (var m in c.CodeMethods)
                {
                    foreach (var v in m.CodeVariableDeclarationStatements)
                    {
                        if (v.Name.ToLower().Contains("secret") || v.Name.ToLower().Contains("password") || v.Name.ToLower().Contains("passwd"))
                        {
                            if (v.Code.Contains("=") && v.Code.Contains("\""))
                            {
                                string message = string.Format("Potential Hardcoded password: {0}", v.Code);
                                retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Orange, "Hardcoded Password"));
                            }
                        }
                    }
                }
            }


            return retval;
        }
    }
}
