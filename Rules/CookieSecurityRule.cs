using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public class CookieSecurityRule : IRule
    {
        private FileAnalyzer analyzer;

        public CookieSecurityRule(FileAnalyzer a)
        {
            this.analyzer = a;
        }

        public List<IVulnerability> Test()
        {
            List<IVulnerability> retval = new List<IVulnerability>();
            string raw = this.analyzer.Raw;
            string[] lines = this.analyzer.Lines;


            if (raw.Contains("HttpCookie"))
            {

                for (int x = 0; x < lines.Length; x++)
                {
                    string currentLine = lines[x];

                    //
                    // This case is: HttpCookie xxx = new HttpCookie("foo", "bar")
                    //               xxx.IsSecure = true;
                    //               xxx.HttpOnly = true; <- looking for these
                    //
                    if (currentLine.Contains("HttpCookie") && currentLine.Contains("=") && currentLine.Contains("new"))
                    {

                        string[] tokens = currentLine.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        for (int tokenIndex = 0; tokenIndex < tokens.Length; tokenIndex++)
                        {
                            if (tokens[tokenIndex].CompareTo("HttpCookie") == 0)
                            {
                                string cookieVariableName = tokens[tokenIndex + 1];

                                if (!raw.Contains(cookieVariableName + ".Secure") && !raw.Contains(cookieVariableName + ".HttpOnly"))
                                {
                                    string message = string.Format("There appears to be an insecurely configured cookie: {0} which does not have .Secure or .HttpOnly configured.\n\n{1}", cookieVariableName, currentLine);
                                    retval.Add(new GenericVulnerability(this.analyzer.Filename, message, System.Drawing.Color.Orange, "Cookie Insecurity"));
                                }
                            }
                        }



                    }
                    else if (currentLine.Contains("HttpCookie(") && currentLine.Contains("new") && !currentLine.Contains("="))
                    {
                        //
                        // This is the case of Response.Cookies.Add( new HttpCookie("foo", "bar"))
                        //

                        if (!currentLine.Contains("true")) // <- it does not appear to have a true flag, so setting to secure is unlikely.
                        {
                            string message = string.Format("There appears to be an insecurely configured cookie which does not have .Secure or .HttpOnly configured.\n\n{0}", currentLine);
                            retval.Add(new GenericVulnerability(this.analyzer.Filename, message, System.Drawing.Color.Orange, "Cookie Insecurity"));
                        }
                    }
                }
            }
               

            return retval;
        }
    }
}
