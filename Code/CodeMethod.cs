using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public class CodeMethod : ICode
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

        public List<CodeVariableInitializer> CodeVariableInitializers
        {
            get;
            private set;
        }

        public List<CodeParameterDeclaration> CodeParameterDeclarations // this is the parameter list for this method.
        {
            get;
            private set;
        }

        public List<CodeAssignmentExpression> CodeAssignmentExpressions
        {
            get;
            private set;
        }

        public CodeMethod(string name, string code)
        {
            this.Name = name;
            this.Code = code;
            this.CodeInvocations = new List<CodeInvocation>();
            this.CodeVariableDeclarationStatements = new List<CodeVariableDeclarationStatement>();
            this.CodeVariableInitializers = new List<CodeVariableInitializer>();
            this.CodeParameterDeclarations = new List<CodeParameterDeclaration>();
            this.CodeAssignmentExpressions = new List<CodeAssignmentExpression>();
        }
    }
}
