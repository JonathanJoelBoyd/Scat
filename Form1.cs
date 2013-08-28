using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Scat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.dlgOpenFolder.Description = "Select the folder at the base of your source code.";
            this.SourceCodeDirectory = string.Empty;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Static Code Analyzer", "scat");
        }

        public string SourceCodeDirectory
        {
            get;
            private set;
        }

        private void SelectSourceCodeDirectory()
        {
            if (dlgOpenFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.SourceCodeDirectory = dlgOpenFolder.SelectedPath;
            }
        }

        private void openSourceFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectSourceCodeDirectory();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.SelectSourceCodeDirectory();
        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.SourceCodeDirectory))
            {
                this.SelectSourceCodeDirectory();
            }

            this.Scan();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.SourceCodeDirectory))
            {
                this.SelectSourceCodeDirectory();

            }

            this.Scan();
        }

        SourceCodeAnalyzer sourceCodeAnalyzer;
        Thread sourceCodeAnalyzerThread;

        private void Scan()
        {
            if (!string.IsNullOrEmpty(this.SourceCodeDirectory))
            {
                this.lblStatus.Text = "Scanning...";
                this.sourceCodeAnalyzer = new SourceCodeAnalyzer(this.SourceCodeDirectory, this.Completed, this.Debug);
                this.sourceCodeAnalyzerThread = new Thread(this.sourceCodeAnalyzer.OnThread);
                this.sourceCodeAnalyzerThread.Start();
            }
        }


        private void Debug(string message)
        {
            this.FindForm().Invoke(new Action(() =>
            {
                rtbDebug.Text += message + "\n";
            }));
        }

        private string GetSeverityForColor(Color c)
        {
            string retval = "Debug";

            if (c == Color.Red) retval = "Critical";
            else if (c == Color.Orange) retval = "Medium";
            else if (c == Color.Yellow) retval = "Low";
            else if (c == Color.Green) retval = "Info";

            return retval;
        }

        private void Completed(SourceCodeAnalyzer analyzer)
        {

            this.FindForm().Invoke(new Action(() =>
            {
                foreach (IVulnerability v in analyzer.Vulnerabilities)
                {
                    string s = GetSeverityForColor(v.GetSeverity());

                    //
                    // add the severity level.
                    //
                    if (!treeViewResults.Nodes.ContainsKey(s))
                    {
                        TreeNode tnSeverityNode = new TreeNode(s);
                        tnSeverityNode.Name = s;
                        tnSeverityNode.BackColor = v.GetSeverity();
                        treeViewResults.Nodes.Add(tnSeverityNode);
                    }

                    //
                    // add the type level.
                    //

                    //treeViewResults.Nodes[s]
                    if (treeViewResults.Nodes[s].Nodes == null)
                    {
                        MessageBox.Show("huh.");
                    }

                    if (!treeViewResults.Nodes[s].Nodes.ContainsKey(v.GetType()))
                    {
                        TreeNode tnTypeNode = new TreeNode(v.GetType());
                        tnTypeNode.Name = v.GetType();
                        tnTypeNode.BackColor = v.GetSeverity();
                        treeViewResults.Nodes[s].Nodes.Add(tnTypeNode);
                    }

                    //
                    // add the actual vuln.
                    //
                    TreeNode vulnNode = new TreeNode(v.GetFilename());
                    vulnNode.BackColor = v.GetSeverity();
                    vulnNode.Tag = v.GetId();
                    treeViewResults.Nodes[s].Nodes[v.GetType()].Nodes.Add(vulnNode);


                }

                this.lblStatus.Text = "Finished.";
            }));
        }

        private void Configure()
        {
            ConfigurationDialog dlgConfigure = new ConfigurationDialog();
            if (DialogResult.OK == dlgConfigure.ShowDialog())
            {
                MessageBox.Show("ok, then.");
                    
            }
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Configure();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.Configure();
        }

        private void treeViewResults_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // MessageBox.Show( this.treeViewResults.SelectedNode

            if (this.treeViewResults.SelectedNode.Tag != null)
            {
                Guid vulnId = (Guid)this.treeViewResults.SelectedNode.Tag;

                foreach (var v in this.sourceCodeAnalyzer.Vulnerabilities)
                {
                    if (v.GetId() == vulnId)
                    {
                        rtbOutput.Text = v.GetType() + "   " + this.GetSeverityForColor(v.GetSeverity()) + "   " + v.GetFilename() + "\n" + v.GetDescription() +"\n\n";
                    }
                }
            }

        }

        private void openInVisualStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeViewResults.SelectedNode.Tag != null)
            {
                Guid vulnId = (Guid)this.treeViewResults.SelectedNode.Tag;

                foreach (var v in this.sourceCodeAnalyzer.Vulnerabilities)
                {
                    if (v.GetId() == vulnId)
                    {
                        System.Diagnostics.Process.Start(v.GetFilename());
                    }
                }
            }
        }




    }
}
