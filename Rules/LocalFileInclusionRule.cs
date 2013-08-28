using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class LocalFileInclusionRule : IRule
    {
        private FileAnalyzer analyzer;

        public LocalFileInclusionRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            if (this.analyzer.Raw.Contains("System.IO"))
            {

                    foreach (var c in this.analyzer.FileSyntaxAnalyzer.CodeClasses)
                    {
                        foreach (var m in c.CodeMethods)
                        {
                            foreach (var i in m.CodeInvocations)
                            {
                                if (i.Code.Contains("Response.WriteFile"))
                                {
                                    if (i.Code.Contains("Request") )
                                    {
                                        string message = string.Format("Potential Local File Inclusion vulnerability: {0}", i.Code);
                                        retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Red, "Local File Inclusion"));
                                    }
                                }
                                else if(i.Code.Contains("Response.Write"))
                                {
                                    foreach (var declStatement in m.CodeVariableDeclarationStatements)
                                    {
                                        if (declStatement.Code.Contains("File.ReadAll") && declStatement.Code.Contains("Request."))
                                        {
                                            if (i.Code.Contains(declStatement.Name))
                                            {
                                                string message = string.Format("Potential File Disclosure vulnerability: {0}", i.Code);
                                                retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Red, "Local File Inclusion"));
                                            }
                                        }

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
