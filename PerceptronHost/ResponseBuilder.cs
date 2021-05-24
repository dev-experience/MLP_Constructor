using MLP_Constructor.Model.EntityDataModel;
using MultyLayerPerceptron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerceptronHost
{
    public class ResponseBuilder
    {
        private readonly RequestProcessor processor;

        public ResponseBuilder(RequestProcessor processor)
        {
            this.processor = processor;
        }

        public string Build(string data)
        {
            var attributes = processor.Recieve(data);

            if (!attributes.ContainsKey("cmd"))
                throw new InvalidOperationException("Комманда отсутствует");
            var cmd = attributes["cmd"];
            return ExecuteCommand(cmd, attributes);
        }
        private string ExecuteCommand(string cmd, Dictionary<string, string> attributes)
        {
            switch (cmd)
            {

                case "get format": return GetFormat(attributes["id"]);
                case "get results": return GetResults(attributes);
                default: return "Unknown command";
            }

        }
        private string GetResults(Dictionary<string, string> attributes)
        {
            int perceptronId = int.Parse(attributes["id"]);
            using (var context = new MLPContext())
            {
                var perceptron = context.WorkedPerceptrons
                    .First(x => x.Id == perceptronId).Perceptron;

                var inputs = perceptron.builder.Inputs;
                Vector inputValues = new Vector(
                    new double[inputs.Length]);
                for (int i = 0; i < inputs.Length; i++)
                {
                    inputValues[i] = double.Parse(attributes[inputs[i].Name]);
                }
                var outputs = perceptron.builder.Outputs;
                StringBuilder response = new StringBuilder();
                var results = perceptron.GetResults(inputValues)[0];
                return string.Join(processor.AttributeSeparator,
                    outputs.Select((x, i) =>
                    $"{x.Name}{processor.KeyValueSeparator}{results[i]}"));
            }
        }
        private string GetFormat(string id)
        {
            int perceptronId = int.Parse(id);
            using (var context = new MLPContext())
            {
                var perceptron = context.WorkedPerceptrons
                    .First(x => x.Id == perceptronId);
                return string.Join(processor.AttributeSeparator, perceptron.Perceptron.builder
                    .Inputs
                    .Select(x => $"{x.Name}{processor.KeyValueSeparator}value")
                    .ToArray());
            }
        }
    }
}
