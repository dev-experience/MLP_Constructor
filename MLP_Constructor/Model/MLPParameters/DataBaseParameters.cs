using MLP_Constructor.Model.DBContext;
using MLP_Constructor.Model.Supported;
using MultyLayerPerceptron;
using MultyLayerPerceptron.CalculatingGraph;
using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class DataBaseParameters : PerceptronParameter
    {
        private const string prefix = "Support_";
        public double TrainDataPart => 0.8;

        public int GetDataCount()
        {
            if (!TryConstructConnectionString(out var connection)) return -1;
            if (!IsTableExist(true)) return -1;
            using (var context = DataBase.Context(connection))
            {
                return context.GetCount(FormattedTableName());
            }
        }
        private IEnumerable<Tuple<Vector, Vector>> GetTrainDataPacket(int[] ids, DataBaseContext context)
        {
            foreach (var item in context
                             .GetInputOutputData(FormattedTableName(),
                             ids.ToList(), Inputs, Outputs))
            {
                yield return item;
            }
        }
        public IEnumerable<Tuple<Vector, Vector>> GetTrainData(int[] ids)
        {
            if (!TryConstructConnectionString(out var connection)) yield break;
            using (var context = DataBase.Context(connection))
            {
                if (ids.Length < 100)
                {
                    foreach (var item in GetTrainDataPacket(ids, context))
                    {
                        yield return item;
                    }
                    yield break;
                }

                
                List<int> miniIds = new List<int>();
                for (int i = 0; i < ids.Length; i++)
                {
                    miniIds.Add(ids[ i]);
                    
                    if (i % 100 == 0)
                    {
                        foreach (var item in GetTrainDataPacket(miniIds.ToArray(), context))
                        {
                            yield return item;
                        }
                        miniIds.Clear();
                    }
                }
                foreach (var item in GetTrainDataPacket(miniIds.ToArray(), context))
                {
                    yield return item;
                }

            }
        }

        public int Postfix { get; set; }
        public List<InputParameters> Inputs { get; set; }
        public List<OutputParameters> Outputs { get; set; }
        public string OldOutputClassName { get; set; }
        public string OutputClassName
        {
            get => OldOutputClassName;
            set
            {
                if (OldOutputClassName != value)
                {
                    Outputs.Clear();
                    OldOutputClassName = value;
                }
            }
        }
        public string Provider { get; set; }
        public string OldSource { get; set; }
        public string Source
        {
            get => OldSource;
            set
            {
                if (OldSource != value)
                {
                    Inputs.Clear();
                    Outputs.Clear();
                    OldSource = value;
                }
            }
        }
        public string RowTableName { get; set; }
        public string TableName
        {
            get => RowTableName;
            set
            {
                var name = ToRowTableName(value);
                if (RowTableName != name)
                {
                    Outputs.Clear();
                    Inputs.Clear();
                    RowTableName = name;
                }
            }
        }


        private string ToRowTableName(string name)
        {
            if (name.StartsWith(prefix))
            {
                return Regex.Replace(name, @"(\W*)", "").Replace(prefix, "");
            }
            else
            {
                return name;
            }
        }
        public string FormattedTableName()
        {
            string postf = Postfix > 0 ? $" ({Postfix})" : "";
            return prefix + TableName + postf;
        }
        public DataBaseParameters()
        {
            Outputs = new List<OutputParameters>();
            Inputs = new List<InputParameters>();
        }
        public async Task NormalizeAsync()
        {
            await Task.Run(NormalizeInputs);
        }

        private void NormalizeInputs()
        {
            if (!TryConstructConnectionString(out var connection)) return;
            using (var context = DataBase.Context(connection))
            {
                Inputs = context.GetNormalizedInputs(FormattedTableName(), Inputs);
            }
        }

        public async Task RelocateAsync()
        {
            var task = Task.Run(Relocate);
            await task;
        }
        private void Relocate()
        {
            if (!TryConstructConnectionString(out var connection)) return;
            using (var context = DataBase.Context(connection))
            {
                List<IDbColumn> cols = new List<IDbColumn>();
                cols.AddRange(Inputs);
                cols.AddRange(Outputs);
                context.RelocateData(TableName,
                    FormattedTableName(),
                    OutputClassName,
                    Inputs.ToArray(),
                    Outputs.ToArray());
            }
        }
        public void CreateTable()
        {

            if (!TryConstructConnectionString(out var connection)) return;
            using (var context = DataBase.Context(connection))
            {
                List<IDbColumn> cols = new List<IDbColumn>();
                cols.AddRange(Inputs);
                cols.AddRange(Outputs);
                Postfix = context.CreateSupportTable(prefix, RowTableName, cols.ToArray());
            }
        }
        public void CollapseOutputs(string toAdd, string toRemove)
        {
            AddOutputAlternative(toAdd, toRemove);
            RemoveOutput(toRemove);
        }
        public void AddOutputAlternative(string name, string newName)
        {
            var newNameOutput = FindOutput(newName);
            var output = FindOutput(name);
            if (output is null || output.Equals(newNameOutput))
            {
                output = new OutputParameters(name);
                Outputs.Add(output);
            }
            output.AddName(newName);
            if (!(newNameOutput is null))
            {
                output.AddNameRange(newNameOutput.AlternativeNames.ToArray());
            }
            output.ClearAlternatives();
        }
        private OutputParameters FindOutput(string name)
        {
            return Outputs.FirstOrDefault(x => x.Name == name);

        }
        public void RemoveOutput(string name)
        {
            var output = FindOutput(name);
            if (output is null) return;
            Outputs.Remove(output);
        }
        public IEnumerable<string> GetOutputVariants()
        {
            if (OutputClassName is null) yield break;
            if (!TryConstructConnectionString(out var connection)) yield break;
            using (var context = DataBase.Context(connection))
            {
                if (context.IsTableWithColumnsExist(TableName))
                {

                    foreach (var variantName in context.GetOutputVariants(TableName, OutputClassName))
                    {
                        yield return variantName;
                    }
                }
            }
        }
        public bool IsCorrectSource()
        {
            return File.Exists(Source);
        }
        public IEnumerable<string> GetColumnNames()
        {
            if (!TryConstructConnectionString(out var connection)) yield break;

            using (var context = DataBase.Context(connection))
            {
                foreach (var columnName in context.GetColumnNames(TableName))
                {
                    yield return columnName;
                }
            }
        }

        public IEnumerable<string> GetTableNames()
        {
            if (!TryConstructConnectionString(out var connection)) yield break;

            using (var context = DataBase.Context(connection))
            {
                foreach (var tableName in context.GetTableNames())
                {
                    yield return tableName;
                }
            }
        }
        public bool IsTableExist(bool support)
        {
            if (!TryConstructConnectionString(out var connectionString))
            {
                return false;
            }

            List<string> columnsNames = Inputs.Select(x => x.Name).ToList();
            if (support)
            {
                columnsNames.AddRange(Outputs.Select(x => x.Name).ToList());
            }
            else
            {
                columnsNames.Add(OutputClassName);
            }

            using (var context = DataBase.Context(connectionString))
            {
                if (support)
                {
                    return context.IsTableWithColumnsExist(FormattedTableName(), columnsNames.ToArray());
                }
                else
                {
                    return context.IsTableWithColumnsExist(TableName, columnsNames.ToArray());
                }
            }
        }
        protected override bool CheckCorrect()
        {
            if (!IsAvaliableProvider()) return false;
            if (TableName is null) return false;
            if (Outputs.Any(x => !x.IsCorrect) && OutputClassName is null) return false;
            if (Inputs.Any(x => !x.IsCorrect)) return false;
            if (!File.Exists(Source)) return false;
            if (!IsAvaliableProvider()) return false;
            if (TrainDataPart < 0 || TrainDataPart > 1) return false;
            return true;
        }

        public IEnumerable<Output> GetConstructedOutputs()
        {
            foreach (var item in Outputs.Select(x => x.Name))
            {
                yield return new Output(item);
            }
        }
        public IEnumerable<Input> GetConstructedInputs()
        {

            foreach (var item in Inputs)
            {
                var input = new Input<double>(item.MinValue, item.MaxValue, new DoubleNormalizeProvider());
                input.Name = item.Name;
                yield return input;
            }
        }
        public void RemoveInput(string name)
        {
            var input = Inputs.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (input is null) return;
            Inputs.Remove(input);
        }
        public void TryAddInput(string name)
        {
            if (Inputs.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            Inputs.Add(new InputParameters(name));
        }

        private bool TryConstructConnectionString(out string connectionString)
        {
            if (IsAvaliableProvider())
            {

                connectionString = $"Provider={Provider};Data Source={Source};";
                return true;
            }
            else
            {
                connectionString = null;
                return false;
            }
        }
        private bool IsAvaliableProvider()
        {
            if (Source is null) return false;
            if (!File.Exists(Source)) return false;
            if (ProviderSelector.TrySelect(Path.GetExtension(Source), out var provider))
            {
                Provider = provider;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
