using MLP_Constructor.Model.EntityDataModel;
using MLP_Constructor.Model.MLPParameters;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.DBContext
{
    public class DataBaseContext : IDisposable
    {
        private readonly OleDbConnection db;
        public bool IsTableWithColumnsExist(string tableName, params string[] columnsNames)
        {
            throw new NotImplementedException();
        }
        public virtual IEnumerable<string> GetTableNames()
        {
            var tables = db.GetSchema("Tables").Rows;
            for (int i = 0; i < tables.Count; i++)
            {
                var name = tables[i][2].ToString();
                if (name.StartsWith("MSys")) continue;
                yield return tables[i][2].ToString();
            }
        }
        public virtual IEnumerable<string> GetOutputVariants(string tableName, string outputClassName)
        {
            var getQuery = $"SELECT [{outputClassName}] FROM [{tableName}] ";
            var reader = new OleDbCommand(getQuery, db).ExecuteReader();
            string name = "";
            List<string> existedNames = new List<string>();
            while (reader.Read())
            {
                name = reader[0].ToString();
                if (existedNames.Contains(name)) continue;
                existedNames.Add(name);
                yield return name;
            }
        }
        
        public virtual IEnumerable<string> GetColumnNames(string tableName)
        {
            var columns = db.GetSchema("Columns").Rows;
            for (int i = 0; i < columns.Count; i++)
            {
                var name = columns[i][3].ToString();
                if (tableName.Equals(columns[i][2].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    yield return name;
                }
            }
        }
        public DataBaseContext(string connectionString)
        {
            db = new OleDbConnection(connectionString);
            db.Open();
        }
        public void Dispose()
        {
            db.Close();
        }


    }
}
