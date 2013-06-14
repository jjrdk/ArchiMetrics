using System;
using System.Collections.Generic;
using System.Linq;
using ArchiCop.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiMate.Core
{
    [TestClass]
    public class VisualStudioProjectGraphTest
    {
        [TestMethod]
        public void CanCreateEmptyGraph1()
        {
            //

            //
            var graph = new VisualStudioProjectGraph();

            //
            Assert.IsTrue(graph.Vertices.Count == 0);
            Assert.IsTrue(graph.Edges.Count == 0);
        }

        [TestMethod]
        public void CanCreateEmptyGraph2()
        {
            //
            var projects = new List<VisualStudioProjectRoot>();
            //
            var graph = new VisualStudioProjectGraph(projects);

            //
            Assert.IsTrue(graph.Vertices.Count == 0);
            Assert.IsTrue(graph.Edges.Count == 0);
        }

        [TestMethod]
        public void CanCreateGraphWithOnlyVertices()
        {
            //
            var projects = new List<VisualStudioProjectRoot>();
            for (int i = 0; i < 10; i++)
            {
                projects.Add(new VisualStudioProjectRoot(Guid.NewGuid().ToString(), "testproject" + i));
            }
            //
            var graph = new VisualStudioProjectGraph(projects);

            //
            Assert.IsTrue(graph.Vertices.Count == 10);
            Assert.IsTrue(graph.Edges.Count == 0);
        }

        [TestMethod]
        public void CanCreateGraphWithEdges1()
        {
            //
            var projects = new List<VisualStudioProjectRoot>();
            for (int i = 0; i < 3; i++)
            {
                projects.Add(new VisualStudioProjectRoot(Guid.NewGuid().ToString(), "testproject" + i));
            }
            for (int i = 0; i < 3; i++)
            {
                var proj = new VisualStudioProjectRoot(Guid.NewGuid().ToString(), "roottestproject" + i);
                proj.Projects.AddRange(
                    projects.Where(item => item.ProjectName.StartsWith("testproject")).Select(item => item));
                projects.Add(proj);
            }
            //
            var graph = new VisualStudioProjectGraph(projects);

            //
            Assert.IsTrue(graph.Vertices.Count == 6);
            Assert.IsTrue(graph.Edges.Count == 9);
        }

        [TestMethod]
        public void CanCreateGraphWithEdges2()
        {
            //
            var projects = new List<VisualStudioProjectRoot>();

            for (int i = 0; i < 3; i++)
            {
                var proj = new VisualStudioProjectRoot(Guid.NewGuid().ToString(), "roottestproject" + i);

                proj.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bf7", "testproject1"));
                proj.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bf8", "testproject2"));
                proj.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bf9", "testproject3"));

                projects.Add(proj);
            }
            //
            var graph = new VisualStudioProjectGraph(projects);

            //
            Assert.IsTrue(graph.Vertices.Count == 6);
            Assert.IsTrue(graph.Edges.Count == 9);
        }

        [TestMethod]
        public void CanCreateGraphWithEdges2NotCaseSensitiveId()
        {
            //
            var projects = new List<VisualStudioProjectRoot>();

            for (int i = 0; i < 2; i++)
            {
                var proj = new VisualStudioProjectRoot(Guid.NewGuid().ToString(), "roottestproject" + i);

                proj.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bf7", "testproject1"));
                proj.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bf8", "testproject2"));
                proj.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bf9", "testproject3"));

                projects.Add(proj);
            }
            var proj3 = new VisualStudioProjectRoot(Guid.NewGuid().ToString(), "roottestproject3");

            proj3.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bF7", "testproject1"));
            proj3.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bF8", "testproject2"));
            proj3.Projects.Add(new VisualStudioProjectRoot("6612d71d-b527-4a6a-bf09-30f3f1275bF9", "testproject3"));

            projects.Add(proj3);
            //
            var graph = new VisualStudioProjectGraph(projects);

            //
            Assert.IsTrue(graph.Vertices.Count == 6);
            Assert.IsTrue(graph.Edges.Count == 9);
        }
    }
}