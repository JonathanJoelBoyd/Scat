using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
   // <astnode_to_string>s = Request.QueryString ["hello"]</astnode_to_string>
// <type>AssignmentExpression</type>

    public class CodeAssignmentExpression : ICode
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

        public CodeAssignmentExpression(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }
    }
}
