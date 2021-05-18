using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Network
{
    public class HiddenLayer
    {
        private readonly PerceptronBuilder perceptron;

        public HiddenLayer(PerceptronBuilder perceptron)
        {
            this.perceptron = perceptron;
        }
        private int GetLastSize()
        {
            var lastSize = perceptron.Inputs.Length;
            if (perceptron.Weights.Count != 0)
            {
                lastSize = perceptron.Weights.Last().Columns;
            }
            return lastSize;
        }
        private bool CheckCorrectWeights(Matrix weights)
        {
            return weights.Rows == GetLastSize();
        }
        public HiddenLayer AddHidden(int size)
        {

            return AddHidden(Matrix.Rand(GetLastSize(), size, -1, 1));
        }
        public HiddenLayer AddHidden(Matrix weights)
        {
            if (!CheckCorrectWeights(weights))
            {
                throw new ArgumentException($"Некорректный размер матрицы весов." +
                    $"Количество строк должно равняться количеству нейронов с предыдущего слоя.");
            }
            perceptron.Weights.Add(weights);
            return new HiddenLayer(perceptron);
        }
        public Perceptron AddOutput(params Output[] outputs)
        {
            if (outputs is null) throw new ArgumentNullException(nameof(outputs));
            perceptron.Weights.Add(Matrix.Rand(GetLastSize(), outputs.Length, -1, 1));
            perceptron.Outputs = outputs;
            return new Perceptron(perceptron);
        }
        public Perceptron AddOutput(Matrix weights, params Output[] outputs)
        {
            if (!CheckCorrectWeights(weights))
            {
                throw new ArgumentException($"Некорректный размер матрицы весов." +
                  $"Количество строк должно равняться количеству нейронов с предыдущего слоя.");
            }
            if (outputs is null) throw new ArgumentNullException(nameof(outputs));
            if (weights.Columns != outputs.Length)
            {
                throw new ArgumentException($"Некорректный размер матрицы весов." +
                  $"Количество столбцов должно равняться количеству выходных нейронов.");
            }
            perceptron.Weights.Add(weights);
            perceptron.Outputs = outputs;

            return new Perceptron(perceptron);
        }
    }
}
