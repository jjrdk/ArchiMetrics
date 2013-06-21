using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace ArchiCop.Data
{
    public class ExcelInfoRepository : IInfoRepository
    {
        private string _connectionString;

        public ExcelInfoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region IInfoRepository Members

        public IEnumerable<DataSourceInfo> DataSources()
        {
            var dataSourceInfos = new List<DataSourceInfo>();

            IEnumerable<string> dataSourceNames = GetDataSourceNames();

            foreach (string dataSourceName in dataSourceNames)
            {
                var dataSourceInfo = new DataSourceInfo();

                DataSourceRow dataSourceRow = GetDataSourceData(dataSourceName);

                dataSourceInfo.DisplayName = dataSourceName;
                dataSourceInfo.DataSourceName = dataSourceName;

                dataSourceInfo.LoadEngine = new LoadEngineInfo
                    {
                        EngineName = dataSourceRow.LoadEngineType,
                        Arg1 = dataSourceRow.Arg1,
                        Arg2 = dataSourceRow.Arg2
                    };

                dataSourceInfos.Add(dataSourceInfo);
            }

            return dataSourceInfos;
        }

        public IEnumerable<GraphInfo> Graphs()
        {
            var graphInfos = new List<GraphInfo>();

            IEnumerable<string> graphNames = GetGraphNames();

            foreach (string graphName in graphNames)
            {
                var graphInfo = new GraphInfo {GraphName = graphName};

                IEnumerable<GraphRow> graphRows = GetGraphData(graphName);

                GraphRow loadEngineGraphRow = graphRows.First(item => item.RuleType == "LoadEngine");
                graphInfo.LoadEngine = new LoadEngineInfo
                    {
                        EngineName = loadEngineGraphRow.RuleValue,
                        Arg1 = loadEngineGraphRow.Arg1,
                        Arg2 = loadEngineGraphRow.Arg2
                    };

                GraphRow displayNameGraphRow = graphRows.First(item => item.RuleType == "DisplayName");
                graphInfo.DisplayName = displayNameGraphRow.RuleValue;

                IEnumerable<GraphRow> ruleGraphRows = graphRows.Where(item => item.RuleType == "VertexRegexRule");
                foreach (GraphRow ruleGraphRow in ruleGraphRows)
                {
                    var rule = new GraphRuleInfo
                        {
                            RuleType = ruleGraphRow.RuleType,
                            RulePattern = ruleGraphRow.RulePattern,
                            RuleValue = ruleGraphRow.RuleValue
                        };
                    graphInfo.Rules.Add(rule);
                }

                graphInfos.Add(graphInfo);
            }

            return graphInfos;
        }

        private IEnumerable<string> GetGraphNames()
        {
            return GetGraphData().GroupBy(item => item.GraphName).Select(item => item.Key);
        }

        private IEnumerable<GraphRow> GetGraphData(string graphName)
        {
            return GetGraphData().Where(item => item.GraphName == graphName);
        }

        private IEnumerable<GraphRow> GetGraphData()
        {
            var data = new List<GraphRow>();
            IEnumerable<string> graphNames =
                GetExcelSheetNames(_connectionString).Where(item => item.StartsWith("Graph"));

            foreach (string graphName in graphNames)
            {
                data.AddRange(GetGraphDataPage(graphName));
            }

            return data;
        }

        public IEnumerable<string> GetDataSourceNames()
        {
            return GetDataSourceData().GroupBy(item => item.DataSourceName).Select(item => item.Key);
        }

        private DataSourceRow GetDataSourceData(string dataSourceName)
        {
            return GetDataSourceData().First(item => item.DataSourceName == dataSourceName);
        }

        private IEnumerable<DataSourceRow> GetDataSourceData()
        {
            var data = new List<DataSourceRow>();
            IEnumerable<string> dataSourceNames =
                GetExcelSheetNames(_connectionString).Where(item => item.StartsWith("DataSource"));

            foreach (string dataSourceName in dataSourceNames)
            {
                data.AddRange(GetDataSourceDataPage(dataSourceName));
            }

            return data;
        }

        private IEnumerable<DataSourceRow> GetDataSourceDataPage(string tableName)
        {
            _connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                                "Data Source=" + _connectionString + ";Extended Properties=Excel 8.0;";

            var oleDbCon = new OleDbConnection(_connectionString);

            oleDbCon.Open();

            string sql = "SELECT DataSourceName, LoadEngineType, Arg1, Arg2 from [" + tableName + "]";

            var oleDa = new OleDbDataAdapter(sql, _connectionString);
            var ds = new DataSet();
            oleDa.Fill(ds);

            oleDbCon.Close();

            Func<DataRow, DataSourceRow> createrow =
                row => new DataSourceRow
                    {
                        LoadEngineType = row["LoadEngineType"] as string,
                        Arg1 = row["Arg1"] as string,
                        Arg2 = row["Arg2"] as string,
                        DataSourceName = row["DataSourceName"] as string
                    };

            IEnumerable<DataSourceRow> data = from DataRow row in ds.Tables[0].Rows select createrow(row);

            return data;
        }

        private IEnumerable<GraphRow> GetGraphDataPage(string tableName)
        {
            _connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                                "Data Source=" + _connectionString + ";Extended Properties=Excel 8.0;";

            var oleDbCon = new OleDbConnection(_connectionString);

            oleDbCon.Open();

            string sql = "SELECT Arg1, Arg2, RuleType, RuleValue, RulePattern from [" + tableName + "]";

            var oleDa = new OleDbDataAdapter(sql, _connectionString);
            var ds = new DataSet();
            oleDa.Fill(ds);

            oleDbCon.Close();

            Func<DataRow, GraphRow> createGraphInfo =
                row => new GraphRow
                    {
                        RuleType = row["RuleType"] as string,
                        RuleValue = row["RuleValue"] as string,
                        RulePattern = row["RulePattern"] as string,
                        Arg1 = row["Arg1"] as string,
                        Arg2 = row["Arg2"] as string,
                        GraphName = tableName
                    };

            IEnumerable<GraphRow> data = from DataRow row in ds.Tables[0].Rows select createGraphInfo(row);

            return data;
        }

        #endregion

        private IEnumerable<string> GetExcelSheetNames(string connectionString)
        {
            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                               "Data Source=" + connectionString + ";Extended Properties=Excel 8.0;";

            // Create connection object by using the preceding connection string.
            var oleDbCon = new OleDbConnection(connectionString);
            // Open connection with the database.
            oleDbCon.Open();

            // Get the data table containg the schema guid.
            DataTable dt = oleDbCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            var excelSheets = new String[dt.Rows.Count];
            int i = 0;

            // Add the sheet name to the string array.
            foreach (DataRow row in dt.Rows)
            {
                excelSheets[i] = row["TABLE_NAME"].ToString();
                i++;
            }

            oleDbCon.Close();

            return excelSheets;
        }

        private class DataSourceRow
        {
            public string DataSourceName { get; set; }
            public string LoadEngineType { get; set; }
            public string Arg1 { get; set; }
            public string Arg2 { get; set; }
        }

        private class GraphRow
        {
            public string GraphName { get; set; }
            public string RuleType { get; set; }
            public string RuleValue { get; set; }
            public string RulePattern { get; set; }
            public string Arg1 { get; set; }
            public string Arg2 { get; set; }
        }
    }
}