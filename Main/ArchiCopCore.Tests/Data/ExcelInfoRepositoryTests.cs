using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiCop.Data
{
    [TestClass]
    public class ExcelInfoRepositoryTests
    {
        [TestMethod]
        public void GetGraphData_HasCorrectNumberOfRows()
        {
            //
            IInfoRepository repository=new ExcelInfoRepository(@"Data\Sample.xls");

            //
            var graphData = repository.GetGraphData();

            //
            Assert.AreEqual(graphData.Count(), 20);
        }

        [TestMethod]
        public void GetGraphData_HasCorrectNumberOfGraphs()
        {
            //
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            var graphData = repository.GetGraphData();

            //
            Assert.AreEqual(graphData.GroupBy(item=>item.GraphName).Count(), 4);
        }

        [TestMethod]
        public void GetGraphData_HasCorrectGraphs_GraphName()
        {
            //
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            var graphData = repository.GetGraphData();

            //
            string[] source = graphData.GroupBy(item=>item.GraphName).Select(item=>item.Key).ToArray();
            var target = new[] { "GraphArchiCop1$", "GraphArchiCop2$", "GraphTest1$", "GraphTest2$" };
            Assert.IsTrue(source.Intersect(target).Any());
        }

        [TestMethod]
        public void GetDataSourceData_HasCorrectNumberOfRows()
        {
            //
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            var dataSourceData = repository.GetDataSourceData();

            //
            Assert.AreEqual(dataSourceData.Count(),2);
        }
    }
}
