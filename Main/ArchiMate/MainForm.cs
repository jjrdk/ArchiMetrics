using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ArchiMate
{
    public partial class MainForm : Form
    {
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
                    new List<string>(Directory.GetFiles(textBox1.Text, "*csproj", SearchOption.AllDirectories));

                var graph = new VisualStudioProjectGraph(projectsFileNames);

                dataGridView1.DataSource = graph.Vertices.OrderBy(item => item.ProjectName).ToList();
                tabPage1.Text = "Vertices (" + graph.Vertices.Count + ")";

                dataGridView2.DataSource =
                    graph.Edges.Select(
                        item =>
                        new {item.Id, Source = item.Source.ProjectName, Target = item.Target.ProjectName}).ToList();
                tabPage2.Text = "Edges (" + graph.Edges.Count + ")";
            }

        }
    }
}