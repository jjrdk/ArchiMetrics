using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

                _graph = new VisualStudioProjectGraph(projectsFileNames);                

                BindGrids();
            }

        }

        void BindGrids()
        {
            var edges = _graph.Edges;

            if (!checkBox1.Checked)
            {
                edges = edges.Where(item => item.Target.VertexType != ".csproj").ToList();
            }
            if (!checkBox2.Checked)
            {
                edges = edges.Where(item => item.Target.VertexType != ".fsproj").ToList();
            }
            if (!checkBox3.Checked)
            {
                edges = edges.Where(item => item.Target.VertexType != ".vbproj").ToList();
            }

            edges = edges.Where(item => Regex.IsMatch(item.Source.VertexName, textBox2.Text))
                    .Where(item => Regex.IsMatch(item.Target.VertexName, textBox3.Text)).ToList();


            var graph = new VisualStudioProjectGraph();

            foreach (Edge<VisualStudioProject> edge in edges)
            {
                graph.AddEdge(edge.Source,edge.Target);
            }

            dataGridView2.DataSource = graph.Edges.Select(item =>
                new { item.Id, Source = item.Source.VertexName, Target = item.Target.VertexName, TargetVertexType = item.Target.VertexType }).ToList();
            tabPage2.Text = "Edges (" + edges.Count + ")";

            var vertices = graph.Vertices;

            dataGridView1.DataSource = vertices.OrderBy(item => item.VertexName).ToList();
            tabPage1.Text = "Vertices (" + vertices.Count + ")";
           
        }

       

    }
}