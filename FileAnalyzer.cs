using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scat
{
    public class FileAnalyzer
    {
        public string Filename
        {
            get;
            private set;
        }

        public string Raw
        {
            get;
            private set;
        }

        public string[] Lines
        {
            get;
            private set;
        }

        public List<IVulnerability> Vulnerabilities
        {
            get;
            private set;
        }

        public FileAnalyzer(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Cannot file: " + fileName);
            }

            this.Filename = fileName;
            this.Raw = File.ReadAllText(this.Filename);
            this.Lines = File.ReadAllLines(this.Filename);
            this.Vulnerabilities = new List<IVulnerability>();
            this.Load(); // load the syntax tree incase we need it.
        }

        public FileSyntaxAnalyzer FileSyntaxAnalyzer
        {
            get;
            private set;
        }

        private void Load()
        {
            this.FileSyntaxAnalyzer = new FileSyntaxAnalyzer(this.Filename);
            this.FileSyntaxAnalyzer.Analyze(); // now we have a syntax tree for this file.
        }

        public void Analyze()
        {
            // for each rule, do the rule and grab the vulns.

            List<IRule> rules = new List<IRule>();
            //rules.Add(new TestRule(this));
            
            
            rules.Add(new AspxXSSRule(this));
            rules.Add(new ScaryAspxRule(this));
            rules.Add(new CookieSecurityRule(this));
            rules.Add(new ScaryCsRule(this));
            rules.Add(new LocalFileInclusionRule(this));
            rules.Add(new HardCodedPasswordRule(this));
            rules.Add(new DomBasedXssRule(this));
            rules.Add(new HashWithoutSaltRule(this));
            rules.Add(new WeakHashRule(this));

            foreach (var r in rules)
            {
                List<IVulnerability> vulns = r.Test();

                foreach (var v in vulns)
                {
                    this.Vulnerabilities.Add(v);
                }
            }
        }
    }
}
