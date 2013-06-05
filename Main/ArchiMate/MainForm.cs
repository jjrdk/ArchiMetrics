using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ArchiMate.Data;

namespace ArchiMate
{
    public partial class MainForm : Form
    {
        private VisualStudioProjectGraph _graph;

        public MainForm()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox1.Text))
            {
                MessageBox.Show("Directory " + textBox1.Text + " doesn't exist!");
            }

            else
            {
                var projectsFileNames =
                    new List<string>(Directory.GetFiles(textBox1.Text, "*fsproj", SearchOption.AllDirectories));
                
                projectsFileNames.AddRange(
                    new List<string>(Directory.GetFiles(textBox1.Text, "*csproj", SearchOption.AllDirectories)));

                projectsFileNames.AddRange(
                    new List<string>(Directory.GetFiles(textBox1.Text, "*vbproj", SearchOption.AllDirectories)));

                _graph = new VisualStudioProjectGraph(new VisualStudioProjectRepository().GetProjects(projectsFileNames));                

                BindGrids();
            }

        }

        void BindGrids()
        {
            var edges = _graph.Edges;

            if (!checkBox1.Checked)
            {
                edges = edges.Where(item => item.Source.Data.ProjectType != ".csproj").ToList();
                edges = edges.Where(item => item.Target.Data.ProjectType != ".csproj").ToList();
            }
            if (!checkBox2.Checked)
            {
                edges = edges.Where(item => item.Source.Data.ProjectType != ".fsproj").ToList();
                edges = edges.Where(item => item.Target.Data.ProjectType != ".fsproj").ToList();
            }
            if (!checkBox3.Checked)
            {
                edges = edges.Where(item => item.Source.Data.ProjectType != ".vbproj").ToList();
                edges = edges.Where(item => item.Target.Data.ProjectType != ".vbproj").ToList();
            }

            edges = edges.Where(item => Regex.IsMatch(item.Source.Name, textBox2.Text))
                    .Where(item => Regex.IsMatch(item.Target.Name, textBox3.Text)).ToList();


            var graph = new VisualStudioProjectGraph();

            foreach (Edge<VisualStudioProject> edge in edges)
            {
                graph.AddEdge(edge.Source,edge.Target);
            }

            dataGridView2.DataSource = graph.Edges.Select(item =>
                new { item.Id, Source = item.Source.Name, Target = item.Target.Name, TargetVertexType = item.Target.Data.ProjectType }).ToList();
            tabPage2.Text = "References (" + edges.Count + ")";

            var vertices = graph.Vertices;

            dataGridView1.DataSource = vertices.OrderBy(item => item.Name).Select(item => new { Id = item.Id, Name = item.Name, ProjectType = item.Data.ProjectType, ProjectPath = item.Data.ProjectPath }).ToList();
            tabPage1.Text = "Projects (" + vertices.Count + ")";
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BindGrids();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("ArchiMate.Template_sln"));

            string sol = textStreamReader.ReadToEnd();

            var projectIncludes = new StringBuilder();

            /*
            Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ArchiCopCore", "ArchiCopCore\ArchiCopCore.csproj", "{3A4D7180-400E-486E-84B9-ADF89DDB790F}"
            EndProject
            */

            foreach (Vertex<VisualStudioProject> project in _graph.Vertices)
            {
                projectIncludes.AppendLine(string.Format("Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{0}\", \"{1}\", \"{{{2}}}\"", project.Name, project.Data.ProjectPath, project.Id));
                projectIncludes.AppendLine("EndProject");
            }

            sol = sol.Replace("{projects}", projectIncludes.ToString());

            using (var file = new StreamWriter(new FileStream(@"C:\temp\Sample.Sln",FileMode.Create)))
            {
                file.Write(sol);
            }

        }

       

    }
}