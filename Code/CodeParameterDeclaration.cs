using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    // <astnode_to_string>string a</astnode_to_string>
    public class CodeParameterDeclaration : ICode
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
            set;
        }

        public CodeParameterDeclaration(string name, string code)
        {
            this.Name = name;
            this.Code = code;
            this.bTainted = false;
        }

    }
}
