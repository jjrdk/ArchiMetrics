using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace ArchiCop.Core
{
    public class LoadEngineExcel : LoadEngine
    {
        private readonly string _connString;
        private readonly string _excelsheetname;

        public LoadEngineExcel(string excelFile, string excelsheetname)
        {
            _connString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                          "Data Source=" + excelFile + ";Extended Properties=Excel 8.0;";

            _excelsheetname = excelsheetname;
        }

        protected override IEnumerable<ArchiCopEdge<ArchiCopVertex>> GetEdges()
        {
            var oleDbCon = new OleDbConnection(_connString);

            oleDbCon.Open();

            string sql = "SELECT Source, Target from [" + _excelsheetname + "]";

            var oleDa = new OleDbDataAdapter(sql, _connString);
            var ds = new DataSet();
            oleDa.Fill(ds);

            oleDbCon.Close();

            Func<DataRow, ArchiCopEdge<ArchiCopVertex>> newEdge =
                row =>
                new ArchiCopEdge<ArchiCopVertex>(new ArchiCopVertex(row["Source"] as string),
                                                 new ArchiCopVertex(row["Target"] as string));

            return (from DataRow row in ds.Tables[0].Rows select newEdge(row)).ToList();
        }
    }
}