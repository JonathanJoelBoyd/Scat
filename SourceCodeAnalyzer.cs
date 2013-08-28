using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Scat
{
    public class SourceCodeAnalyzer
    {
        public string SourceCodeDirectory
        {
            get;
            private set;
        }

        private Action<SourceCodeAnalyzer> completed;
        private Action<string> debug;

        public List<string> SourceFiles
        {
            get;
            private set;
        }

        public SourceCodeAnalyzer(string sourceCodeDirectory, Action<SourceCodeAnalyzer> completed, Action<string> debug)
        {
            if(string.IsNullOrEmpty(sourceCodeDirectory))
            {
                throw new ArgumentNullException("sourceCodeDirectory");
            }

            SourceCodeDirectory = sourceCodeDirectory;
            this.completed = completed;
            this.debug = debug;
            this.SourceFiles = new List<string>();
            this.FileAnalyzers = new List<FileAnalyzer>();
            this.Vulnerabilities = new List<IVulnerability>();
        }

        private static bool filtered(string filename)
        {
            bool retval = false;

            string lwr = filename.ToLower();

            if (lwr.Contains("crm"))
            {
                if (lwr.Contains("qa") || lwr.Contains("test") || lwr.Contains("tool"))
                {
                    retval = true;
                }
            }

            return retval;
        }

        private void EnumerateFiles()
        {
            this.debug("Discovering source files.");
            IEnumerable<string> hFiles = Directory.EnumerateFiles(this.SourceCodeDirectory, "*.h", SearchOption.AllDirectories);
            IEnumerable<string> cppFiles = Directory.EnumerateFiles(this.SourceCodeDirectory, "*.cpp", SearchOption.AllDirectories);
            IEnumerable<string> csFiles = Directory.EnumerateFiles(this.SourceCodeDirectory, "*.cs", SearchOption.AllDirectories);
            IEnumerable<string> aspxFiles = Directory.EnumerateFiles(this.SourceCodeDirectory, "*.aspx", SearchOption.AllDirectories);
            IEnumerable<string> sqlFiles = Directory.EnumerateFiles(this.SourceCodeDirectory, "*.sql", SearchOption.AllDirectories);

            IEnumerable<string> allFiles = csFiles.Union(aspxFiles).Union(hFiles).Union(cppFiles).Union(sqlFiles);

            foreach (string sourceFile in allFiles)
            {
                if (!filtered(sourceFile))
                {
                    this.SourceFiles.Add(sourceFile);
                    
                }
            }


            this.debug("Finished discovering source files. Discovered: " + this.SourceFiles.Count);
        }

        public List<FileAnalyzer> FileAnalyzers
        {
            get;
            private set;
        }

        public List<IVulnerability> Vulnerabilities
        {
            get;
            private set;
        }

        public void OnThread()
        {
            this.EnumerateFiles();

            this.debug("Loading source files...");
            foreach (string file in this.SourceFiles)
            {
                if (!filtered(file))
                {
                    FileAnalyzers.Add(new FileAnalyzer(file));
                }
            }
            this.debug("Finished loading source files (" + FileAnalyzers.Count + " loaded).");


            this.debug("Starting file analysis...");
            Parallel.ForEach(FileAnalyzers, currentAnalyzer =>
            {
                currentAnalyzer.Analyze();
            });
            this.debug("Finished file analysis.");

            this.debug("Aggregating results...");
            foreach (var analyzer in FileAnalyzers)
            {
                foreach (var vuln in analyzer.Vulnerabilities)
                {
                    this.Vulnerabilities.Add(vuln);
                }
            }
            this.debug("Finished aggregating results.");

            //
            //
            //

            this.debug("Starting experimental GlobalScaryRule test code...");
            List<IRule> globalRules = new List<IRule>();
            globalRules.Add(new GlobalScaryCsRule(this.FileAnalyzers, this.debug));
            globalRules.Add(new TraceGlobalRule(this.FileAnalyzers, this.debug));

            foreach (IRule r in globalRules)
            {
                List<IVulnerability> vulns = r.Test();
                foreach (var v in vulns)
                {
                    this.Vulnerabilities.Add(v);
                }
            }
            

            this.completed(this);
        }
    }
}
