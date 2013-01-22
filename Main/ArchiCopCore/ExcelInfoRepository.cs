using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace ArchiCop
{
    public class ExcelInfoRepository : IInfoRepository
    {
        private readonly string _connString;

        public ExcelInfoRepository(string excelFile)
        {
            _connString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                          "Data Source=" + excelFile + ";Extended Properties=Excel 8.0;";
        }

        public GraphInfo GetGraphInfoData(string excelsheetname)
        {
            var oleDbCon = new OleDbConnection(_connString);

            oleDbCon.Open();

            string sql = "SELECT Arg1, Arg2, RuleType, RuleValue, RulePattern from [" + excelsheetname + "]";

            var oleDa = new OleDbDataAdapter(sql, _connString);
            var ds = new DataSet();
            oleDa.Fill(ds);

            oleDbCon.Close();

            Func<DataRow, InfoData> createGraphInfo =
                row => new InfoData
                    {
                        RuleType = row["RuleType"] as string,
                        RuleValue = row["RuleValue"] as string,
                        RulePattern = row["RulePattern"] as string,
                        Arg1 = row["Arg1"] as string,
                        Arg2 = row["Arg2"] as string,
                    };

            IEnumerable<InfoData> data = from DataRow row in ds.Tables[0].Rows select createGraphInfo(row);

            var graphInfo = new GraphInfo();

            if (data.FirstOrDefault(item => item.RuleType == "LoadEngine") != default(InfoData))
            {
                graphInfo.LoadEngine = data.FirstOrDefault(item => item.RuleType == "LoadEngine").RuleValue;
                graphInfo.Arg1 = data.FirstOrDefault(item => item.RuleType == "LoadEngine").Arg1;
                graphInfo.Arg2 = data.FirstOrDefault(item => item.RuleType == "LoadEngine").Arg2;
            }

            if (data.FirstOrDefault(item => item.RuleType == "DisplayName") != default(InfoData))
            {
                graphInfo.DisplayName = data.FirstOrDefault(item => item.RuleType == "DisplayName").RuleValue;
            }

            var vertexRegexRules = new List<VertexRegexRule>();
            foreach (InfoData rule in data.Where(item => item.RuleType == "VertexRegexRule"))
            {
                vertexRegexRules.Add(new VertexRegexRule {Pattern = rule.RulePattern, Value = rule.RuleValue});
            }
            graphInfo.VertexRegexRules = vertexRegexRules;

            return graphInfo;
        }

        /// <summary>
        ///     This method retrieves the excel sheet names from
        ///     an excel workbook.
        /// </summary>
        public IEnumerable<string> GetGraphNames()
        {
            // Create connection object by using the preceding connection string.
            var oleDbCon = new OleDbConnection(_connString);
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

            return excelSheets.Where(item => item.StartsWith("Graph"));
        }

        private class InfoData
        {
            public string RuleType { get; set; }
            public string RuleValue { get; set; }
            public string RulePattern { get; set; }
            public string Arg1 { get; set; }
            public string Arg2 { get; set; }
        }
    }
}