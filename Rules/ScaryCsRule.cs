using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class ScaryCsRule : IRule
    {
        private FileAnalyzer analyzer;

        public ScaryCsRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            if (analyzer.Filename.EndsWith(".cs"))
            {

                foreach (var c in this.analyzer.FileSyntaxAnalyzer.CodeClasses)
                {
                    foreach (var m in c.CodeMethods)
                    {
                        foreach (var i in m.CodeInvocations)
                        {
                            foreach (var scaryMethod in Util.ScaryMethodNames)
                            {


                                //
                                // this is the case where the invocation has a Request user-controlled input.
                                //
                                if (i.Code.Contains(scaryMethod + " ("))
                                {
                                    if (i.Code.Contains("Request"))
                                    {
                                        string message = string.Format("Potential scary method invocation: {0} in the following code:\n\n{1}", scaryMethod, i.Code);
                                        retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Red, "Scary Method Invocation"));
                                    }
                                    else
                                    {
                                        foreach (var f in c.CodeFields)
                                        {
                                            if (f.bTainted)
                                            {
                                                if (i.Code.Contains(f.Name))
                                                {
                                                    string message = string.Format("Potential scary method invocation: {0} with a class field parameter: {1} in the following code:\n\n{2}", scaryMethod, f.Code, i.Code);
                                                    retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Red, "Scary Method Invocation"));
                                                }
                                            }
                                        }

                                        foreach (var x in c.CodeVariableInitializers)
                                        {
                                            if (x.bTainted)
                                            {
                                                if (i.Code.Contains(x.Name))
                                                {
                                                    string message = string.Format("Potential scary method invocation: {0} with tainted variable: {1} in the following code:\n\n{2}", scaryMethod, x.Code, i.Code);
                                                    retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Red, "Scary Method Invocation"));
                                                }
                                            }
                                        }

                                        foreach (var x in m.CodeVariableInitializers)
                                        {
                                            if (x.bTainted)
                                            {
                                                if (i.Code.Contains(x.Name))
                                                {
                                                    string message = string.Format("Potential scary method invocation: {0} with tainted variable: {1} in the following code:\n\n{2}", scaryMethod, x.Code, i.Code);
                                                    retval.Add(new GenericVulnerability(this.analyzer.Filename, message, Color.Red, "Scary Method Invocation"));
                                                }
                                            }
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
