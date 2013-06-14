using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiMate.Core
{
    [TestClass]
    public class GraphTest
    {
        [TestMethod]
        public void CanCreateEmptyGraph1()
        {
            //

            //
            var graph = new Graph<string>();

            //
            Assert.IsTrue(graph.Vertices.Count == 0);
            Assert.IsTrue(graph.Edges.Count == 0);
        }

        [TestMethod]
        public void CanAddOnlyVerticesToGraph()
        {
            //
            var graph = new Graph<string>();

            //            
            for (int i = 0; i < 10; i++)
            {
                graph.AddVertex(new Vertex<string>(Guid.NewGuid().ToString(), "v" + i));
            }

            //
            Assert.IsTrue(graph.Vertices.Count == 10);
            Assert.IsTrue(graph.Edges.Count == 0);
        }

        [TestMethod]
        public void CanAddOnlyVerticesToGraphNotCaseSensitive()
        {
            //
            var graph = new Graph<string>();

            //            
            for (int i = 0; i < 10; i++)
            {
                graph.AddVertex(new Vertex<string>("v" + i, "v" + i));
            }
            for (int i = 0; i < 10; i++)
            {
                graph.AddVertex(new Vertex<string>("V" + i, "V" + i));
            }

            //
            Assert.IsTrue(graph.Vertices.Count == 10);
            Assert.IsTrue(graph.Edges.Count == 0);
        }

        [TestMethod]
        public void CanAddOnlyEdgesToGraph()
        {
            //
            var graph = new Graph<string>();
            //
            var root = new Vertex<string>("root", "root");

            for (int i = 0; i < 3; i++)
            {
                var target = new Vertex<string>("t" + i, "t" + i);
                graph.AddEdge(root, target);
            }

            //
            Assert.IsTrue(graph.Vertices.Count == 4);
            Assert.IsTrue(graph.Edges.Count == 3);
        }

        [TestMethod]
        public void CanAddOnlyEdgesToGraphNotCaseSensitive()
        {
            //
            var graph = new Graph<string>();
            //
            var root = new Vertex<string>("root", "root");

            for (int i = 0; i < 3; i++)
            {
                var target = new Vertex<string>("t" + i, "t" + i);
                graph.AddEdge(root, target);
            }

            for (int i = 0; i < 3; i++)
            {
                var target = new Vertex<string>("T" + i, "T" + i);
                graph.AddEdge(root, target);
            }

            //
            Assert.IsTrue(graph.Vertices.Count == 4);
            Assert.IsTrue(graph.Edges.Count == 3);
        }
    }
}