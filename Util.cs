using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    public static class Util
    {
        public static string StateStackToString(Stack<State> s)
        {
            string retval = string.Empty;
            State[] a = s.ToArray();

            foreach (var state in a)
            {
                retval += state.ToString() + "\n";
            }

            return retval;
        }

        public static string[] ScaryMethodNames = { 
            "File.AppendAllLines",
            "File.AppendAllText",
            "File.AppendText",
            "File.Copy",
            "File.Exists",
            "File.Create",
            "File.Decrypt",
            "File.Delete",
            "File.Encrypt",
            "File.Move",
            "File.ReadAllBytes",
            "File.ReadAllLines",
            "File.ReadAllText",
            "File.ReadLines",
            "File.SetAccessControl",
            "File.SetCreationTime",
            "File.WriteAllBytes",
            "File.WriteAllLines",
            "File.WriteAllText",
            "SqlCommand",
            "Server.Execute",
            "Server.Transfer",
            "Directory.CreateDirectory",
            "Directory.Delete",
            "Directory.Move",
            "Response.Write",
            "Response.TransmitFile",
            "Response.WriteFile",
            "Response.BinaryWrite",
            "Response.Redirect",
            "Process.Start"
                               };


        public static string ParseMethodNameFromInvocation(string invocationCode)
        {
            string retval = string.Empty;


            string sub = invocationCode.Substring(0, invocationCode.IndexOf('('));

            string[] tokens = sub.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                retval = token;
            }


            return retval.Trim();
        }


        //
        // finds all instances of <% ... %>
        //
        public static IEnumerable<string> FindAllInlineCodes(string raw)
        {
            List<string> retval = new List<string>();

            if (raw.Contains("<%") && raw.Contains("%>"))
            {
                char[] pch = raw.ToCharArray();

                bool open = false;
                StringBuilder sb = new StringBuilder();

                for (int x = 0; x < pch.Length; x++)
                {
                    if (open)
                    {
                        sb.Append(pch[x]);
                    }

                    if (pch[x] == '<' && pch[x + 1] == '%')
                    {
                        open = true;
                    }
                    else if (pch[x] == '%' && pch[x + 1] == '>')
                    {
                        open = false;
                        retval.Add("<" + sb.ToString() + ">");
                        sb = new StringBuilder();
                    }

                }
            }


            return retval;
        }


    }
}
