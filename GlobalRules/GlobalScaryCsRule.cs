using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class GlobalScaryCsRule : IRule
    {
        public List<FileAnalyzer> FileAnalyzers
        {
            get;
            private set;
        }

        Action<string> debug;

        List<Tuple<State, string>> scaryEdges;
        List<Tuple<State, State>> edges;

        public GlobalScaryCsRule(List<FileAnalyzer> fileAnalyzers, Action<string> debug)
        {
            this.FileAnalyzers = fileAnalyzers;
            this.debug = debug;
            this.scaryEdges = GlobalUtil.BuildScaryGraph(this.FileAnalyzers);
            this.edges = GlobalUtil.BuildGraph(this.FileAnalyzers);
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            List<IVulnerability> l = this.EvilRequestPassedAsParameterToMethodThatCallsScaryMethod();

            foreach (var v in l)
            {
                retval.Add(v);
            }


            List<IVulnerability> l2 = this.EvilAssignedToVariablePassedToMethodThatCallsAScaryMethod();
            foreach (var v in l2)
            {
                retval.Add(v);
            }

            List<IVulnerability> l3 = this.EvilTest();
            foreach (var v in l3)
            {
                retval.Add(v);
            }

            return retval;
        }

        public List<IVulnerability> EvilTest()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            foreach (var edge in this.edges)
            {
                foreach (var scaryEdge in this.scaryEdges)
                {
                    if (edge.Item2.ToString().CompareTo(scaryEdge.Item1.ToString()) == 0)
                    {
                        //
                        // edge.Item1 calls edge.Item2.
                        // edge.Item2 == scaryEdge.Item1
                        // Therefore: edge.Item1->Edge.Item2->scaryEdge.Item1->scaryMethod.
                        //
                        // now, let's see if any of edge's things have request.
                        //

                        foreach (var v in edge.Item1.CodeMethod.CodeVariableDeclarationStatements)
                        {
                            if (v.bTainted)
                            {
                                // this is a tainted variable.  Is it passed as a parameter to edge.Item2

                                foreach (var i in edge.Item1.CodeMethod.CodeInvocations)
                                {
                                    if (i.Code.Contains(v.Name) && i.Code.Contains(edge.Item2.CodeMethod.Name))
                                    {
                                        string t = DoesThisMethodPassParametersToAScaryMethod(edge.Item2);
                                        if (!string.IsNullOrEmpty(t))
                                        {
                                            retval.Add(new GenericVulnerability(edge.Item1.FileAnalyzer.Filename, "TEST: " + t, Color.Red, "Evil"));
                                        }
                                        else
                                        {
                                            t = DoesThisMethodAssignParametersToALocalVariableWhichIsPassedToAScaryMethod(edge.Item2);
                                            if (!string.IsNullOrEmpty(t))
                                            {
                                                retval.Add(new GenericVulnerability(edge.Item1.FileAnalyzer.Filename, "TEST: " + t, Color.Red, "Evil"));
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

        public string DoesThisMethodPassParametersToAScaryMethod(State s)
        {
            string retval = string.Empty;

            foreach (var scaryEdge in this.scaryEdges)
            {
                if (scaryEdge.Item1.ToString().CompareTo(s.ToString()) == 0)
                {
                    //
                    // ok, it *calls* a scary method, but does it pass a parameter to it?
                    //

                    foreach (var invocation in s.CodeMethod.CodeInvocations)
                    {
                        if (invocation.Code.Contains(scaryEdge.Item2))
                        {
                            foreach (var p in scaryEdge.Item1.CodeMethod.CodeParameterDeclarations)
                            {
                                if (invocation.Code.Contains(p.Name))
                                {
                                    //
                                    // it appears to do this.
                                    //

                                    retval = string.Format("{0} calls {1} and passes in parameter {2}", s.ToString(), invocation.Name, p.Name);


                                }
                            }
                        }
                    }

                }
            }

            return retval;
        }

        public string DoesThisMethodAssignParametersToALocalVariableWhichIsPassedToAScaryMethod(State s)
        {
            string retval = string.Empty;

            foreach (var scaryEdge in this.scaryEdges)
            {
                if (scaryEdge.Item1.ToString().CompareTo(s.ToString()) == 0)
                {
                    //
                    // ok, s calls a scary method.  Let's see what it does with parameters.
                    //

                    foreach (var p in scaryEdge.Item1.CodeMethod.CodeParameterDeclarations)
                    {
                        foreach (var variableDeclaration in scaryEdge.Item1.CodeMethod.CodeVariableDeclarationStatements)
                        {
                            if (variableDeclaration.Code.Contains(p.Name))
                            {
                                //
                                // we appear to be assigning parameter 'p' to variableDeclaration. Do we pass variableDeclaration.Name
                                //

                                foreach (var invocation in scaryEdge.Item1.CodeMethod.CodeInvocations)
                                {
                                    if (invocation.Code.Contains(scaryEdge.Item2))
                                    {
                                        if (invocation.Code.Contains(variableDeclaration.Name))
                                        {
                                            //
                                            // we are passing in a parameter.  That parameter is getting assigned.  The assigned variable is getting passed to evil.
                                            //

                                            retval = string.Format("{0} is passed a parameter {1} which is assigned to a local variable {2} which is passed to scary method: {3}",
                                                s.ToString(), p.Name, variableDeclaration.Name, scaryEdge.Item2);

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

        public List<IVulnerability> EvilAssignedToVariablePassedToMethodThatCallsAScaryMethod()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            foreach (var edge in this.edges)
            {
                foreach (var codeAssignmentExpression in edge.Item1.CodeMethod.CodeAssignmentExpressions)
                {
                    if (codeAssignmentExpression.Code.Contains("Request"))
                    {
                        //
                        // s = Request.QueryString["foo"].
                        // See if s is passed to any of our invocations.
                        //

                        foreach (var invocation in edge.Item1.CodeMethod.CodeInvocations)
                        {
                            if (invocation.Code.Contains(codeAssignmentExpression.Name))
                            {
                                //
                                // we've assigned evil to a variable and passed that variable to another method.
                                // let's find that method and see if he does anything stupid with it.
                                //


                                State destination = edge.Item2;
                                //if(destination.CodeMethod.Name.Contains
                                if (invocation.Code.Contains(destination.CodeMethod.Name))
                                {
                                    // we appear to be calling this method and passing in the tainted variable.
                                    // does this method call a scary method?

 


                                    foreach (var scaryEdge in this.scaryEdges)
                                    {
                                        if (scaryEdge.Item1.ToString().CompareTo(destination.ToString()) == 0)
                                        {
                                            //
                                            // destination appears to be calling a scary method.  
                                            // we're passing taint to destination.  Let's see if destination
                                            // is passing that to a scary method, or assigning it to a local variable an
                                            //

                                            foreach (var scaryInvocation in scaryEdge.Item1.CodeMethod.CodeInvocations)
                                            {
                                                if (scaryInvocation.Code.Contains(scaryEdge.Item2))
                                                {
                                                    foreach (var p in scaryEdge.Item1.CodeMethod.CodeParameterDeclarations)
                                                    {
                                                        //if(scaryInvocation.Code.Contains(
                                                        if (scaryInvocation.Code.Contains(p.Name))
                                                        {
                                             

                                                            string message = string.Format("{0} \n\ncalls\n\n {1}\n\n passing in \n\n{2}\n\n which was initialized using user input here \n\n{3}. \n\nThis invokes \n\n{4}\n\n which is here: \n\n{5}.\n\n This calls a scary method \n\n{6}\n\n passing in parameter \n\n{7}", 
                                                                edge.Item1.ToString(), 
                                                                edge.Item2.ToString(),
                                                                codeAssignmentExpression.Name,
                                                                codeAssignmentExpression.Code,
                                                                invocation.Name,
                                                                scaryEdge.Item1.ToString(),
                                                                scaryEdge.Item2,
                                                                p.Name);

                                                            retval.Add(new GenericVulnerability(edge.Item1.FileAnalyzer.Filename, message, Color.Red, "Evil"));

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
                }
            }

            return retval;
        }

        public List<IVulnerability> EvilRequestPassedAsParameterToMethodThatCallsScaryMethod()
        {
            List<IVulnerability> retval = new List<IVulnerability>();

            foreach (var t in edges)
            {
                //
                // go through the starting
                //

                foreach (var i in t.Item1.CodeMethod.CodeInvocations)
                {
                    if (i.Code.Contains("Request"))
                    {
                        State destination = t.Item2;

                        //this.debug(t.Item1.ToString() + " ------------ " + t.Item2.ToString() + " ===================== " + i.Code);

                        foreach (var scaryEdge in scaryEdges)
                        {
                            if (scaryEdge.Item1.ToString().Contains(destination.ToString())) // we're calling a dangerous method
                            {
                                foreach (var ci in scaryEdge.Item1.CodeMethod.CodeInvocations)
                                {
                                    if (ci.Code.Contains(scaryEdge.Item2))
                                    {
                                        foreach (var p in scaryEdge.Item1.CodeMethod.CodeParameterDeclarations)
                                        {
                                            if (ci.Code.Contains(p.Name))
                                            {
                                                string message = t.Item1.ToString() + "->\n" + scaryEdge.Item1.ToString() + "->\n" + scaryEdge.Item2 + "\n";
                                                message += i.Code + "\n" + ci.Code;

                                                retval.Add(new GenericVulnerability(t.Item1.FileAnalyzer.Filename, message, Color.Red, "Evil"));

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
