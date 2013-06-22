using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiCop.Data
{
    [TestClass]
    public class ExcelInfoRepositoryTests
    {
        [TestMethod]
        public void HasCorrectNumberOfGraphInfos()
        {
            //
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<GraphInfo> data = repository.ConfigInfo.Graphs;

            //
            Assert.AreEqual(data.Count(), 4);
        }

        [TestMethod]
        public void HasCorrectNumberOfDataSourceInfos()
        {
            //
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<DataSourceInfo> data = repository.ConfigInfo.DataSources;

            //
            Assert.AreEqual(data.Count(), 2);
        }

        [TestMethod]
        public void GraphInfosHaveCorrectDisplayNames()
        {
            //
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<GraphInfo> data = repository.ConfigInfo.Graphs;

            //
            string[] source = data.Select(item => item.DisplayName).ToArray();
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
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<DataSourceInfo> data = repository.ConfigInfo.DataSources;

            //
            string[] source = data.Select(item => item.DisplayName).ToArray();
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
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<GraphInfo> data = repository.ConfigInfo.Graphs;

            //
            string[] source = data.Select(item => item.DataSource.LoadEngine.EngineName).ToArray();
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
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<DataSourceInfo> data = repository.ConfigInfo.DataSources;

            //
            string[] source = data.Select(item => item.LoadEngine.EngineName).ToArray();
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
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<GraphInfo> data = repository.ConfigInfo.Graphs;

            //
            string[] source = data.Select(item => item.DataSource.LoadEngine.Arg1).ToArray();
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
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<DataSourceInfo> data = repository.ConfigInfo.DataSources;

            //
            string[] source = data.Select(item => item.LoadEngine.Arg1).ToArray();
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
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<GraphInfo> data = repository.ConfigInfo.Graphs;

            //
            string[] source = data.Select(item => item.DataSource.LoadEngine.Arg2).ToArray();
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
            IInfoRepository repository = new ExcelInfoRepository(@"Data\Sample.xls");

            //
            IEnumerable<DataSourceInfo> data = repository.ConfigInfo.DataSources;

            //
            string[] source = data.Select(item => item.LoadEngine.Arg2).ToArray();
            var target = new[]
                {
                    null,
                    "DataTest$"
                };
            Assert.IsTrue(source.Intersect(target).Any());
        }
    }
}