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

        public IEnumerable<GraphRow> GetGraphData()
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

        public IEnumerable<DataSourceRow> GetDataSourceData()
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


        //public IEnumerable<string> GetDataSourceNames()
        //{
        //    return GetExcelSheetNames(_connectionString).Where(item => item.StartsWith("Data"));
        //}

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
    }
}