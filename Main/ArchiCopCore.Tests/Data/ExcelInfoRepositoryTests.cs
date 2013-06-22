using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiCop.Data
{
    [TestClass]
    public class ExcelInfoRepositoryTests
    {
        readonly IEnumerable<GraphInfo> _graphs = 
            new ExcelInfoRepository().GetConfigInfos(@"Data\Sample.xls").First().Graphs;

        readonly IEnumerable<DataSourceInfo> _dataSources = 
            new ExcelInfoRepository().GetConfigInfos(@"Data\Sample.xls").First().DataSources;

        [TestMethod]
        public void HasCorrectNumberOfGraphInfos()
        {
            //
            Assert.AreEqual(_graphs.Count(), 4);
        }

        [TestMethod]
        public void HasCorrectNumberOfDataSourceInfos()
        {
            //
            Assert.AreEqual(_dataSources.Count(), 2);
        }

        [TestMethod]
        public void GraphInfosHaveCorrectDisplayNames()
        {
            //
            string[] source = _graphs.Select(item => item.DisplayName).ToArray();
            var target = new[]
                {
                    "ArchiCop1",
                    "ArchiCop2",
                    "Graph Test 1",
                    "Graph Test 2"
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }

        [TestMethod]
        public void DataSourceInfosHaveCorrectDisplayNames()
        {            
            //
            string[] source = _dataSources.Select(item => item.DisplayName).ToArray();
            var target = new[]
                {
                    "archicop",
                    "test1",
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }

        [TestMethod]
        public void GraphInfosHaveCorrectLoadEngines_EngineName()
        {            
            //
            string[] source = _graphs.Select(item => item.DataSource.LoadEngine.EngineName).ToArray();
            var target = new[]
                {
                    "ArchiCop.Core.VisualStudioProjectLoadEngine,ArchiCopCore",
                    "ArchiCop.Core.VisualStudioProjectLoadEngine,ArchiCopCore",
                    "ArchiCop.Core.LoadEngineExcel,ArchiCopCore",
                    "ArchiCop.Core.LoadEngineExcel,ArchiCopCore"
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }

        [TestMethod]
        public void DataSourceInfosHaveCorrectLoadEngines_EngineName()
        {
            //
            string[] source = _dataSources.Select(item => item.LoadEngine.EngineName).ToArray();
            var target = new[]
                {
                    "ArchiCop.Core.VisualStudioProjectLoadEngine,ArchiCopCore",
                    "ArchiCop.Core.LoadEngineExcel,ArchiCopCore",
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }

        [TestMethod]
        public void GraphInfosHaveCorrectLoadEngines_Arg1()
        {
            //
            string[] source = _graphs.Select(item => item.DataSource.LoadEngine.Arg1).ToArray();
            var target = new[]
                {
                    @"..\..\..",
                    @"..\..\..",
                    "Sample.xls",
                    "Sample.xls"
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }

        [TestMethod]
        public void DataSourceInfosHaveCorrectLoadEngines_Arg1()
        {
            //
            string[] source = _dataSources.Select(item => item.LoadEngine.Arg1).ToArray();
            var target = new[]
                {
                    @"..\..\..",
                    "Sample.xls"
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }

        [TestMethod]
        public void GraphInfosHaveCorrectLoadEngines_Arg2()
        {
            //
            string[] source = _graphs.Select(item => item.DataSource.LoadEngine.Arg2).ToArray();
            var target = new[]
                {
                    null,
                    null,
                    "DataTest$",
                    "DataTest$"
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }

        [TestMethod]
        public void DataSourceInfosHaveCorrectLoadEngines_Arg2()
        {
            //
            string[] source = _dataSources.Select(item => item.LoadEngine.Arg2).ToArray();
            var target = new[]
                {
                    null,
                    "DataTest$"
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }
    }
}