using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp;
using System.IO;
namespace Scat
{
    public class FileSyntaxAnalyzer
    {
        public string Filename
        {
            get;
            private set;
        }

        public string RawCode
        {
            get;
            private set;
        }

        public SyntaxTree SyntaxTree
        {
            get;
            private set;
        }


        public List<CodeClass> CodeClasses
        {
            get;
            private set;
        }

        public FileSyntaxAnalyzer(string filename)
        {
            this.Filename = filename;
            this.RawCode = File.ReadAllText(this.Filename);
            this.CodeClasses = new List<CodeClass>();
        }

        public void Analyze()
        {
            if (this.Filename.EndsWith(".cs"))
            {
                this.SyntaxTree = SyntaxTree.Parse(this.RawCode);
                Analyze(this.SyntaxTree.Children);
            }
        }

        private void Analyze(IEnumerable<AstNode> nodes)
        {
            foreach (AstNode node in nodes)
            {
                string code = node.ToString();
                string typeName = node.GetType().Name;

                if (typeName.CompareTo("TypeDeclaration") == 0)
                {
                    CodeClass codeClass = new CodeClass(FindNext(node.Children, "Identifier"), code);


                    CodeClasses.Add(codeClass);
                }
                else if (typeName.CompareTo("FieldDeclaration") == 0)
                {
                    CodeField f = new CodeField(FindNext(node.Children, "Identifier"), code);
                    CodeClasses.Last().CodeFields.Add(f);
                }
                else if (typeName.CompareTo("MethodDeclaration") == 0)
                {
                    CodeMethod m = new CodeMethod(FindNext(node.Children, "Identifier"), code);
                    CodeClasses.Last().CodeMethods.Add(m);
                }
                else if (typeName.CompareTo("InvocationExpression") == 0)
                {
                    CodeInvocation i = new CodeInvocation(Util.ParseMethodNameFromInvocation(code), code);
                    if (CodeClasses.Count > 0)
                    {
                        if (CodeClasses.Last().CodeMethods.Count > 0)
                        {
                            CodeClasses.Last().CodeMethods.Last().CodeInvocations.Add(i);
                        }
                        else
                        {
                            CodeClasses.Last().CodeInvocations.Add(i);
                        }

                    }
                }
                else if (typeName.CompareTo("VariableDeclarationStatement") == 0)
                {

                    CodeVariableDeclarationStatement s = new CodeVariableDeclarationStatement(FindNext(node.Children, "Identifier"), code);
                    if (CodeClasses.Count > 0)
                    {
                        if (CodeClasses.Last().CodeMethods.Count > 0)
                        {
                            CodeClasses.Last().CodeMethods.Last().CodeVariableDeclarationStatements.Add(s);
                        }
                        else
                        {
                            CodeClasses.Last().CodeVariableDeclarationStatements.Add(s);
                        }
                    }
                }
                else if (typeName.CompareTo("VariableInitializer") == 0)
                {

                    CodeVariableInitializer s = new CodeVariableInitializer(FindNext(node.Children, "Identifier"), code);
                    //
                    if (CodeClasses.Last().CodeMethods.Count > 0)
                    {
                        CodeClasses.Last().CodeMethods.Last().CodeVariableInitializers.Add(s);
                    }
                    else
                    {
                        CodeClasses.Last().CodeVariableInitializers.Add(s);
                    }
                }
                else if (typeName.CompareTo("ParameterDeclaration") == 0)
                {
                    try
                    {
                        CodeParameterDeclaration p = new CodeParameterDeclaration(FindNext(node.Children, "Identifier"), code);
                        if (CodeClasses.Last().CodeMethods.Count > 0)
                        {
                            CodeClasses.Last().CodeMethods.Last().CodeParameterDeclarations.Add(p);
                        }
                    }
                    catch (Exception) // masking this.  basically, it's adding a variable to a global namespace but since there aren't any classes defined yet this is problematic.
                    {
                    }
                }
                else if (typeName.CompareTo("AssignmentExpression") == 0)
                {
                    try
                    {
                        CodeAssignmentExpression codeAssignmentExpression = new CodeAssignmentExpression(FindNext(node.Children, "IdentifierExpression"), code);
                        if (CodeClasses.Last().CodeMethods.Count > 0)
                        {
                            CodeClasses.Last().CodeMethods.Last().CodeAssignmentExpressions.Add(codeAssignmentExpression);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }



                if (node.Children.Count() > 0)
                {
                    Analyze(node.Children);
                }
            }
        }

        private string FindNext(IEnumerable<AstNode> nodes, string type)
        {
            string retval = string.Empty;

            foreach (AstNode node in nodes)
            {
                string typeName = node.GetType().Name;

                if (typeName.CompareTo(type) == 0)
                {
                    retval = node.ToString();
                    break;
                }

                if (node.Children.Count() > 0)
                {
                    retval = FindNext(node.Children, type);
                }
            }

            return retval;
        }

    }
}
