using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace ArchiCop.Core
{
    public class LoadEngineExcel : ILoadEngine
    {
        private readonly string _connString;
        private readonly string _excelsheetname;

        public LoadEngineExcel(string excelFile, string excelsheetname)
        {
            _connString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                          "Data Source=" + excelFile + ";Extended Properties=Excel 8.0;";

            _excelsheetname = excelsheetname;
        }

        public IEnumerable<ArchiCopEdge> LoadEdges()
        {
            var edges = new List<ArchiCopEdge>();

            var oleDbCon = new OleDbConnection(_connString);

            oleDbCon.Open();

            string sql = "SELECT Source, Target from [" + _excelsheetname + "]";

            var oleDa = new OleDbDataAdapter(sql, _connString);
            var ds = new DataSet();
            oleDa.Fill(ds);

            oleDbCon.Close();

            var vertices = new List<ArchiCopVertex>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var source = row["Source"] as string;
                var target = row["Target"] as string;

                if (source != null & target != null)
                {

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

                    edges.Add(new ArchiCopEdge(sVertex, tVertex));

                }
            }

            return edges;
        }
    }
}