using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public class State
    {
        public FileAnalyzer FileAnalyzer
        {
            get;
            private set;
        }

        public CodeClass CodeClass
        {
            get;
            private set;
        }

        public CodeMethod CodeMethod
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return this.FileAnalyzer.Filename + ":" + this.CodeClass.Name + ":" + this.CodeMethod.Name;
        }

        public State Copy()
        {
            return new State(this.FileAnalyzer, this.CodeClass, this.CodeMethod);
        }

        public bool IsEvil
        {
            get;
            private set;
        }

        public string ScaryMethodUsed
        {
            get;
            private set;
        }

        public State(FileAnalyzer fileAnalyzer, CodeClass codeClass, CodeMethod codeMethod)
        {
            this.FileAnalyzer = fileAnalyzer;
            this.CodeClass = codeClass;
            this.CodeMethod = codeMethod;
            this.IsEvil = false;

            foreach (string scaryMethod in Util.ScaryMethodNames)
            {
                if (this.CodeMethod.Code.Contains(scaryMethod))
                {
                    this.IsEvil = true;
                    this.ScaryMethodUsed = scaryMethod;
                    break;
                }
            }
        }
    }
}
