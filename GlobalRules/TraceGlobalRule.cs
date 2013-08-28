using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scat
{
    public class TraceGlobalRule : IRule
    {
        public List<FileAnalyzer> FileAnalyzers
        {
            get;
            private set;
        }

        Action<string> debug;

        List<Tuple<State, string>> scaryEdges;
        List<Tuple<State, State>> edges;

        public TraceGlobalRule(List<FileAnalyzer> fileAnalyzers, Action<string> debug)
        {
            this.FileAnalyzers = fileAnalyzers;
            this.debug = debug;
            this.scaryEdges = GlobalUtil.BuildScaryGraph(this.FileAnalyzers);
            this.edges = GlobalUtil.BuildGraph(this.FileAnalyzers);
            this.Vulnerabilities = new List<IVulnerability>();
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();
            List<State> startingStates = GlobalUtil.FindStartingStates(this.FileAnalyzers);

            foreach (var s in startingStates)
            {
                Trace(s, new Stack<State>());   
            }

            foreach (var v in this.Vulnerabilities)
            {
                retval.Add(v);
            }

            return retval;
        }

        public List<IVulnerability> Vulnerabilities
        {
            get;
            private set;
        }

        public void Trace(State s, Stack<State> stack)
        {
            bool isEvilState = false;

            stack.Push(s);

            if (s.IsEvil)
            {
                this.Vulnerabilities.Add(new GenericVulnerability(s.FileAnalyzer.Filename, Util.StateStackToString(stack) + "  " + s.ScaryMethodUsed, Color.LightPink, "TRACE"));
            }
            else
            {
                foreach (var t in this.edges)
                {
                    if (t.Item1.ToString().CompareTo(s.ToString()) == 0)
                    {
                        Trace(t.Item2, stack);
                    }
                }
            }

            stack.Pop(); // remove s
        }

    
       
    }
}
