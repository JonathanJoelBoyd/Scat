using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public static class GlobalUtil
    {
        public static List<State> FindStartingStates(List<FileAnalyzer> FileAnalyzers)
        {
            List<State> retval = new List<State>();

            foreach (var a in FileAnalyzers)
            {
                foreach (var c in a.FileSyntaxAnalyzer.CodeClasses)
                {
                    foreach (var m in c.CodeMethods)
                    {
                        if (m.Code.Contains("Request.") || m.Code.Contains("Request["))
                        {
                            retval.Add(new State(a, c, m));
                        }
                    }
                }
            }

            return retval;
        }

        public static List<Tuple<State, string>> BuildScaryGraph(List<FileAnalyzer> FileAnalyzers)
        {
            List<Tuple<State, string>> retval = new List<Tuple<State, string>>();

            foreach (FileAnalyzer a in FileAnalyzers)
            {
                foreach (CodeClass c in a.FileSyntaxAnalyzer.CodeClasses)
                {
                    foreach (CodeMethod m in c.CodeMethods)
                    {
                        State startState = new State(a, c, m);

                        foreach (CodeInvocation i in m.CodeInvocations)
                        {
                            foreach (string scaryMethod in Util.ScaryMethodNames)
                            {
                                if (i.Code.Contains(scaryMethod))
                                {
                                    retval.Add(new Tuple<State, string>(startState, scaryMethod));
                                }
                            }
                        }
                    }
                }
            }

            return retval;
        }


        public static List<Tuple<State, State>> BuildGraph(List<FileAnalyzer> FileAnalyzers)
        {
            List<Tuple<State, State>> retval = new List<Tuple<State, State>>();

            foreach (FileAnalyzer a in FileAnalyzers)
            {
                foreach (CodeClass c in a.FileSyntaxAnalyzer.CodeClasses)
                {
                    foreach (CodeMethod m in c.CodeMethods)
                    {
                        State startState = new State(a, c, m);

                        foreach (CodeInvocation i in m.CodeInvocations)
                        {
                            State targetState = FindTarget(FileAnalyzers, new State(a, c, m), i.Name);
                            if (targetState != null)
                            {
                                // this.debug(a.Filename + ":" + c.Name + ":" + m.Name + " -> " + targetState.FileAnalyzer.Filename + ":" + targetState.CodeClass.Name + ":" + targetState.CodeMethod.Name);
                                retval.Add(new Tuple<State, State>(startState, targetState));
                            }
                        }
                    }
                }
            }

            return retval;
        }

        public static State FindTarget(List<FileAnalyzer> FileAnalyzers, State s, string invokedMethod)
        {
            State retval = null;

            foreach (FileAnalyzer fileAnalyzer in FileAnalyzers)
            {
                foreach (var c in fileAnalyzer.FileSyntaxAnalyzer.CodeClasses)
                {
                    foreach (var m in c.CodeMethods)
                    {
                        if (m.Name.CompareTo(invokedMethod) == 0)
                        {
                            retval = new State(fileAnalyzer, c, m);
                        }
                    }
                }
            }

            return retval;
        }
    }
}
