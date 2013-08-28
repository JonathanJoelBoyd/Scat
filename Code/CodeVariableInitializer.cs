using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public class CodeVariableInitializer : ICode
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

        public bool bTainted
        {
            get;
            private set;
        }

        public CodeVariableInitializer(string name, string code)
        {
            this.Name = name;
            this.Code = code;
            if (this.Code.Contains("Request.QueryString") || this.Code.Contains("Request.Form"))
            {
                this.bTainted = true;
            }
            else
            {
                this.bTainted = false;
            }
        }
    }
}
