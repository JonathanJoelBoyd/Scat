using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public class CodeInvocation : ICode
    {

        public string GetName()
        {
            return this.Name;
        }

        public string GetCode()
        {
            return this.Code;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Code
        {
            get;
            private set;
        }

        public CodeInvocation(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }
    }
}
