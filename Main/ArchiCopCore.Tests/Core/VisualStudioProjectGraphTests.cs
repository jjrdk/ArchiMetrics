using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiCop.Core
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
            var graph = new VisualStudioProjectGraph(projects);

            //
            Assert.IsTrue(graph.Vertices.Count() == 2);
        }

        [TestMethod]
        public void GraphHasCorrectNumberOfEdges()
        {
            //
            IEnumerable<VisualStudioProject> projects = GetSampleProjects();

            //
            var graph = new VisualStudioProjectGraph( projects );

            //
            Assert.IsTrue(graph.Edges.Count() == 1);
        }

       
    }
}