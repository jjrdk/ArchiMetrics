using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace ArchiCop
{
    public class ExcelLoadEngine : List<ArchiCopEdge>
    {
        private readonly string _connString;

        public ExcelLoadEngine(string excelFile, string excelsheetname)
        {
            _connString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                          "Data Source=" + excelFile + ";Extended Properties=Excel 8.0;";

            CreateEdges(excelsheetname);
        }

        public void CreateEdges(string excelsheetname)
        {
            var oleDbCon = new OleDbConnection(_connString);

            oleDbCon.Open();

            string sql = "SELECT Source, Target from [" + excelsheetname + "]";

            var oleDa = new OleDbDataAdapter(sql, _connString);
            var ds = new DataSet();
            oleDa.Fill(ds);

            oleDbCon.Close();

            var vertices = new List<ArchiCopVertex>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var source = row["Source"] as string;
                var target = row["Target"] as string;

                ArchiCopVertex sVertex = vertices.FirstOrDefault(item => item.Name == source);
                if (sVertex == null)
                {
                    sVertex = new ArchiCopVertex(source);
                    vertices.Add(sVertex);
                }

                ArchiCopVertex tVertex = vertices.FirstOrDefault(item => item.Name == target);
                if (tVertex == null)
                {
                    tVertex = new ArchiCopVertex(target);
                    vertices.Add(tVertex);
                }

                Add(new ArchiCopEdge(sVertex, tVertex));
            }
        }
    }
}