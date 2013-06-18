using System;
using System.Windows.Forms;
using ArchiCop.Core;

namespace ArchiMate
{
    internal class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            IVisualStudioProjectRepository projectRepository = new VisualStudioProjectRepository();
            IVisualStudioSolutionRepository solutionRepository = new VisualStudioSolutionRepository();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(projectRepository, solutionRepository));
        }
    }
}