using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ArchiCop.Core;
using QuickGraph;

namespace ArchiMate
{
    public partial class MainForm : Form
    {
        private readonly IVisualStudioProjectRepository _projectRepository;
        private readonly IVisualStudioSolutionRepository _solutionRepository;
        private VisualStudioProjectGraph _graph;
        private VisualStudioProjectGraph _graphCached;

        public MainForm(IVisualStudioProjectRepository projectRepository,
                        IVisualStudioSolutionRepository solutionRepository)
        {
            InitializeComponent();

            _projectRepository = projectRepository;
            _solutionRepository = solutionRepository;
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

                _graph = new VisualStudioProjectGraph(_projectRepository.GetProjects(projectsFileNames));

                BindGrids();
            }
        }

        private void BindGrids()
        {
            IEnumerable<Edge<VisualStudioProject>> edges = _graph.Edges;

            if (!checkBox1.Checked)
            {
                edges = edges.Where(item => item.Source.ProjectType != ".csproj").ToList();
                edges = edges.Where(item => item.Target.ProjectType != ".csproj").ToList();
            }
            if (!checkBox2.Checked)
            {
                edges = edges.Where(item => item.Source.ProjectType != ".fsproj").ToList();
                edges = edges.Where(item => item.Target.ProjectType != ".fsproj").ToList();
            }
            if (!checkBox3.Checked)
            {
                edges = edges.Where(item => item.Source.ProjectType != ".vbproj").ToList();
                edges = edges.Where(item => item.Target.ProjectType != ".vbproj").ToList();
            }

            edges = edges.Where(item => Regex.IsMatch(item.Source.ProjectName, textBox2.Text))
                         .Where(item => Regex.IsMatch(item.Target.ProjectName, textBox3.Text)).ToList();

            _graphCached = new VisualStudioProjectGraph(edges);
            
            dataGridView2.DataSource = _graphCached.Edges.Select(item =>
                                                                 new
                                                                     {
                                                                         Source = item.Source.ProjectName,
                                                                         Target = item.Target.ProjectName
                                                                     })
                                                   .ToList();
            tabPage2.Text = "References (" + edges.Count() + ")";

            IEnumerable<VisualStudioProject> vertices = _graphCached.Vertices;

            dataGridView1.DataSource =
                vertices.OrderBy(item => item.ProjectName)
                        .Select(
                            item =>
                            new
                                {                                    
                                    item.ProjectName,
                                    item.ProjectType,
                                    item.TargetFrameworkVersion,
                                    item.OutputType,
                                    item.AssemblyName,
                                    item.RootNamespace,
                                    item.ProjectPath,
                                    item.ProjectTypeGuids,
                                    item.ProjectTypes
                                })
                        .ToList();
            tabPage1.Text = "Projects (" + vertices.Count() + ")";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BindGrids();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _solutionRepository.CreateNewSolution(_graphCached, textBox1.Text + @"\Sample.Sln");
        }
    }
}