using MLP_Constructor.Model.Supported;
using MultyLayerPerceptron.CalculatingGraph;
using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class DataBaseParameters : PerceptronParameter
    {

        public double TrainDataPart { get; set; }

        public List<InputParameters> Inputs { get; set; }
        public List<OutputParameters> Outputs { get; set; }
        private string outputClassName;
        public string OutputClassName 
        {
            get => outputClassName;
            set
            {
                if (outputClassName != value)
                {
                    Outputs.Clear();
                    outputClassName = value;
                }
            }
        }
        public string Provider { get; set; }
        public string source;
        public string Source
        {
            get => source;
            set
            {
                if(source != value)
                {
                    Inputs.Clear();
                    Outputs.Clear();
                    source = value;
                }
            }
        }
        public string TableName { get; set; }
        public bool IsSupportTable { get; set; }
        public DataBaseParameters()
        {
            Outputs = new List<OutputParameters>();
            Inputs = new List<InputParameters>();
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
                foreach (var variantName in context.GetOutputVariants(TableName, OutputClassName))
                {
                    yield return variantName;
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
        public bool IsTableExist()
        {
            if (!TryConstructConnectionString(out var connectionString))
            {
                return false;
            }

            List<string> columnsNames = Inputs.Select(x => x.Name).ToList();
            if (IsSupportTable)
            {
                columnsNames.AddRange(Outputs.Select(x => x.Name).ToList());
            }
            else
            {
                columnsNames.Add(OutputClassName);
            }
            string tableName = IsSupportTable ? "Support_" : "";
            tableName += TableName;
            bool isExist = false;
            using (var context = DataBase.Context(connectionString))
            {
                isExist = context.IsTableWithColumnsExist(tableName, columnsNames.ToArray());
            }
            return isExist;
        }
        protected override bool CheckCorrect()
        {
            if (!IsAvaliableProvider()) return false;
            if (TableName is null) return false;
            if (Outputs.Any(x => !x.IsCorrect) && OutputClassName is null) return false;
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
                var input = new Input<double>(item.MinValue, item.MinValue, new DoubleNormalizeProvider());
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
