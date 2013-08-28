using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public class CodeClass : ICode
    {
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

        public List<CodeField> CodeFields
        {
            get;
            set;
        }

        public List<CodeMethod> CodeMethods
        {
            get;
            set;
        }

        public List<CodeVariableInitializer> CodeVariableInitializers
        {
            get;
            private set;
        }

        public List<CodeInvocation> CodeInvocations
        {
            get;
            private set;
        }

        public List<CodeVariableDeclarationStatement> CodeVariableDeclarationStatements
        {
            get;
            private set;
        }

        public CodeClass(string name, string code)
        {
            this.Name = name;
            this.Code = code;
            this.CodeFields = new List<CodeField>();
            this.CodeMethods = new List<CodeMethod>();
            this.CodeVariableInitializers = new List<CodeVariableInitializer>();
            this.CodeInvocations = new List<CodeInvocation>();
            this.CodeVariableDeclarationStatements = new List<CodeVariableDeclarationStatement>();
        }

        public string GetName()
        {
            return this.Name;
        }

        public string GetCode()
        {
            return this.Code;
        }
    }
}
