using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    //    <astnode_to_string>string t = Request.Form ["far"];
    // </astnode_to_string>

    public class CodeVariableDeclarationStatement : ICode
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

        public CodeVariableDeclarationStatement(string name, string code)
        {
            this.Name = name;
            this.Code = code;
            this.bTainted = IsTainted(code);
        }

        private bool IsTainted(string code)
        {
            bool retval = false;


            if (code.Contains("Request.QueryString") || code.Contains("Request.Form"))
            {
                retval = true;
            }

            return retval;
        }
    }
}
