using System.Collections.Generic;
using System.Linq;
using ArchiCop.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiCop.VisualStudioData
{
    [TestClass]
    public class VisualStudioProjectGraphTests : BaseTest
    {
        [TestMethod]
        public void GraphHasCorrectNumberOfVertices()
        {
            //
            IEnumerable<VisualStudioProject> projects = GetSampleProjects();

            //
            ArchiCopGraph<VisualStudioProject> graph = new VisualStudioProjectGraphEngine().GetGraph(projects);

            //
            Assert.IsTrue(graph.Vertices.Count() == 2);
        }

        [TestMethod]
        public void GraphHasCorrectNumberOfEdges()
        {
            //
            IEnumerable<VisualStudioProject> projects = GetSampleProjects();

            //
            ArchiCopGraph<VisualStudioProject> graph = new VisualStudioProjectGraphEngine().GetGraph(projects);

            //
            Assert.IsTrue(graph.Edges.Count() == 1);
        }
    }
}