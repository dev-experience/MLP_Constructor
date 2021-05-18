using MLP_Constructor.Model.EntityDataModel;
using MLP_Constructor.Model.MLPParameters;
using MultyLayerPerceptron;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.DBContext
{
    public class DataBaseContext : IDisposable
    {
        private readonly OleDbConnection db;
        public bool IsTableWithColumnsExist(string tableName, params string[] columnsNames)
        {
            if (!GetTableNames().Contains(tableName)) return false;
            var columns = GetColumnNames(tableName);
            foreach (var item in columnsNames)
            {
                if (!columns.Contains(item)) return false;
            }
            return true;
        }

        internal int GetCount(string tableName)
        {
            string query = $"SELECT COUNT([Id]) FROM [{tableName}]";
            using (var cmd = new OleDbCommand(query, db))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    return int.Parse(reader[0].ToString());
                }
            }
        }

        public IEnumerable<Tuple<Vector, Vector>> GetInputOutputData(string source, List<int> ids, List<InputParameters> inputs, List<OutputParameters> outputs)
        {
            List<Tuple<Vector, Vector>> res = new List<Tuple<Vector, Vector>>();
            var query = $"SELECT " +
                string.Join(", ", inputs.Select(x => x.DbName)) + ", " +
                string.Join(", ", outputs.Select(x => x.DbName)) + " " +
                $"FROM [{source}] " +
                $"WHERE " + string.Join(" OR ", ids.Select(x => $"[Id]={x}"));
            using (var cmd = new OleDbCommand(query, db))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Vector inp = new Vector(new double[inputs.Count]);
                        Vector outp = new Vector(new double[outputs.Count]);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (i < inputs.Count)
                            {
                                inp[i] = double.Parse(reader[i].ToString());
                            }
                            else
                            {
                                outp[i-inp.Length] = double.Parse(reader[i].ToString());
                            }
                        }
                        yield return new Tuple<Vector, Vector>(inp, outp);
                    }
                }
            }
        }

        public void RelocateData(string source, string target, string className, InputParameters[] inputs, OutputParameters[] outputs)
        {
            source = "[" + source + "]";
            target = "[" + target + "]";
            className = "[" + className + "]";
            List<IDbColumn> columns = new List<IDbColumn>();
            columns.AddRange(inputs);
            columns.AddRange(outputs);
            string insert = $"INSERT INTO {target} (" + string.Join(", ", columns.Select(x => x.DbName)) + ") ";
            string selectInp = $"SELECT " + string.Join(", ", inputs.Select(x => $"{source}.{x.DbName}")) + ", ";
            string selectOut = string.Join(", ", outputs.Select(x =>
            {
                x.AlternativeNames.Add(x.Name);
                string[] conditions = x.AlternativeNames
                .Select(y => $"{source}.{className} LIKE \'{y}\'").ToArray();
                return $"IIF({string.Join(" OR ", conditions)}, 1, 0) AS {x.DbName}";
            })) + " ";

            string from = $"FROM {source} ";
            string where = $"WHERE {source}.{className} IS NOT NULL AND " +
                $"{string.Join(" AND ", inputs.Select(x => $"{source}.{x.DbName} IS NOT NULL"))};";

            string query = insert + selectInp + selectOut + from + where;
            using (var cmd = new OleDbCommand(query, db))
            {
                cmd.ExecuteNonQuery();
            }
        }
        private void CreateTable(string name, IDbColumn[] columns)
        {
            string createQuery = $"CREATE TABLE [{name}] (";

            List<string> parameters = new List<string>();
            parameters.Add("[Id] INT IDENTITY PRIMARY KEY");
            foreach (var col in columns)
            {
                parameters.Add($"{col.DbName} {col.DbType}");
            }
            createQuery += string.Join(", ", parameters);
            createQuery += ");";
            using (var cmd = new OleDbCommand(createQuery, db))
            {
                cmd.ExecuteNonQuery();
            }
        }
        public int CreateSupportTable(string prefix, string name, IDbColumn[] columns)
        {
            name = name.Replace(prefix, "");
            string formatted = prefix + name;
            if (!IsTableWithColumnsExist(formatted))
            {
                CreateTable(formatted, columns);
                return 0;
            }
            int postfix = 1;
            while (IsTableWithColumnsExist(formatted + $" ({postfix})"))
            {
                postfix++;
            }
            CreateTable(formatted + $" ({postfix})", columns);
            return postfix;
        }

        public List<InputParameters> GetNormalizedInputs(string source, List<InputParameters> inputs)
        {
            var select = "SELECT " + string.Join(", ", inputs.Select(x => x.DbName)) + " ";
            var from = $"FROM [{source}];";
            var query = select + from;
            using (var cmd = new OleDbCommand(query, db))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < inputs.Count; i++)
                        {
                            inputs[i].SetMinMaxValue(Double.Parse(reader[i].ToString()));
                        }
                    }
                }
            }
            return inputs;
        }

        public virtual IEnumerable<string> GetTableNames()
        {
            var tables = db.GetSchema("Tables").Rows;
            for (int i = 0; i < tables.Count; i++)
            {
                var name = tables[i][2].ToString();
                if (name.StartsWith("MSys") || name.StartsWith("~TMP")) continue;
                yield return tables[i][2].ToString();
            }
        }
        public virtual IEnumerable<string> GetOutputVariants(string tableName, string outputClassName)
        {
            var getQuery = $"SELECT [{outputClassName}] FROM [{tableName}] ";

            string name = "";
            List<string> existedNames = new List<string>();
            using (var cmd = new OleDbCommand(getQuery, db))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = reader[0].ToString();
                        if (existedNames.Contains(name)) continue;
                        existedNames.Add(name);
                        yield return name;
                    }
                }
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
