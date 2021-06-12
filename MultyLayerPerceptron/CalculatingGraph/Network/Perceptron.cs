using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using MultyLayerPerceptron.CalculatingGraph.Nodes;
using MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Network
{
    public class Perceptron
    {
        private ObservedOutputNode observedInput;
        private DataInputNode input;
        private SoftCrossEntropyNode output;
        private SumNode estimation;
        private List<Node> allNodes = new List<Node>();
        private List<WeightsNode> weightsNodes = new List<WeightsNode>();
        private List<ConstantInputNode> regRateNodes = new List<ConstantInputNode>();
        public readonly PerceptronBuilder builder;
        private event Action<object, int> OnBatchSizeChanged;
        private event Action<bool> OnResetData;
        private int HiddenLayers => builder.Weights.Count - 1;
        public Perceptron(PerceptronBuilder builder)
        {
            this.builder = builder;
            CreateCalculatingGraph();
        }
        public void SetBatchSize(int value)
        {
            OnBatchSizeChanged?.Invoke(this, value);
        }
        public Perceptron SetLearningRate(double value)
        {
            builder.LearningRate = value;
            foreach (var weightsNode in weightsNodes)
            {
                weightsNode.LearningRate = value;
            }
            return this;
        }
        public Perceptron SetRegularizationRate(double value)
        {
            builder.RegularizationRate = value;
            foreach (var regRateNode in regRateNodes)
            {
                regRateNode.Initiate(value);
            }
            return this;
        }
        public Vector[] GetResults(params Vector[] inputs)
        {
            SetBatchSize(inputs.Length);
            CheckInputs(inputs);
            Batch outputBatch = CalculateResult(inputs);
            Vector[] res = new Vector[outputBatch.Size];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = outputBatch[i].AsVector;
            }
            return res;
        }
        public void Check(out double error, IEnumerable<Tuple<Vector, Vector>> inputOutputTuple)
        {
            SetBatchSize(inputOutputTuple.Count());
            var inputs = inputOutputTuple.Select(x => x.Item1).ToArray();
            var outputs = inputOutputTuple.Select(x => x.Item2).ToArray();
            CheckInputs(inputs);
            CheckOutputs(outputs);
            input.Initiate(GetNormalize(inputs));
            observedInput.Initiate(outputs);
            var est = estimation.Compute();
            error = output.GetError();
            OnResetData?.Invoke(false);
        }
        public Matrix[] Train(out double error,  IEnumerable<Tuple<Vector, Vector>> inputOutputTuple)
        {
            var inputs = inputOutputTuple.Select(x => x.Item1).ToArray();
            var outputs = inputOutputTuple.Select(x => x.Item2).ToArray();

            int count = inputs.Length;
            SetBatchSize(count);
            CheckInputs(inputs.Take(count).ToArray());
            CheckOutputs(outputs.Take(count).ToArray());
            input.Initiate(GetNormalize(inputs));
            observedInput.Initiate(outputs);
            var est = estimation.Compute();
            try
            {

            for (int i = 0; i < est.Size; i++)
            {
                est[i].Check();
            }
            }catch(Exception e)
            {

            }
            error = output.GetError();
            BackProp();
            OnResetData?.Invoke(false);
            return weightsNodes.Select(x => x.Compute()[0]).ToArray();
        }
        public Matrix[] Train(params Tuple<Vector, Vector>[] inputOutputTuple)
        {
            return Train(out var plug, inputOutputTuple);
        }

        private void Subscribe(IEnumerable<Node> nodes)
        {
            OnResetData += estimation.ResetResult;
            OnBatchSizeChanged += estimation.OnBatchSizeChanged;
            foreach (var node in nodes)
            {
                OnBatchSizeChanged += node.OnBatchSizeChanged;
                OnResetData += node.ResetResult;
            }
        }
        private List<Node> GetAllNodes(Node root)
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(root);
            if (!(root.Left is null))
            {
                var left = root.Left as Node;
                foreach (var item in GetAllNodes(left))
                {
                    if (!nodes.Contains(item))
                    {
                        nodes.Add(item);
                    }
                }
            }
            if (!(root.Right is null))
            {
                var right = root.Right as Node;
                foreach (var item in GetAllNodes(right))
                {
                    if (!nodes.Contains(item))
                    {
                        nodes.Add(item);
                    }
                }
            }
            return nodes;
        }
        private List<SumNode> GetSumsChain()
        {
            var hiddens = HiddenLayers;
            if (hiddens < 1) return new List<SumNode>();
            SumNode[] sums = new SumNode[hiddens];
            sums[0] = new SumNode();
            for (int i = 1; i < sums.Count(); i++)
            {
                sums[i] = new SumNode();
                sums[i].AddAsRight(sums[i - 1]);
            }
            return sums.ToList();

        }
        private List<MatrixFrobeniusPoweredNormNode> GetFrobeniusNodes()
        {
            var frobs = new List<MatrixFrobeniusPoweredNormNode>();
            for (int i = 0; i < HiddenLayers + 1; i++)
            {
                frobs.Add(new MatrixFrobeniusPoweredNormNode());
            }
            return frobs;
        }
        private List<WeightsNode> GetWeightsChain()
        {
            var regNode = new ElementsMultiplicationNode();
            estimation.AddAsRight(regNode);
            var sumsChain = GetSumsChain();
            var frobenius = GetFrobeniusNodes();
            var weights = new List<WeightsNode>();
            var regRateNode = new ConstantInputNode(builder.RegularizationRate);
            regRateNodes.Add(regRateNode);
            regNode.AddAsRight(regRateNode);
            if (sumsChain.Count > 0)
            {
                regNode.AddAsLeft(sumsChain.Last());
                sumsChain[0].AddAsRight(frobenius[0]);
            }
            else
            {
                regNode.AddAsLeft(frobenius[0]);
            }
            for (int i = 0; i < frobenius.Count; i++)
            {
                if (i > 0)
                {
                    sumsChain[i - 1].AddAsLeft(frobenius[i]);
                }
                var weightsNode = new WeightsNode(builder.LearningRate);
                weightsNode.Initiate(builder.Weights[i]);
                frobenius[i].AddAsLeft(weightsNode);
                weights.Add(weightsNode);
                weightsNodes.Add(weightsNode);
            }
            return weights;
        }
        private List<MatrixMultiplicationNode> GetOutputs(SoftCrossEntropyNode softCrossEntropy)
        {
            var matMuls = new List<MatrixMultiplicationNode>();
            var activations = new List<ActivationNode>();
            matMuls.Add(new MatrixMultiplicationNode());
            for (int i = 0; i < HiddenLayers; i++)
            {

                ActivationNode act = new ActivationNode(builder.ActivationFunc);
                MatrixMultiplicationNode matmul = new MatrixMultiplicationNode();
                matmul.AddAsRight(act);
                act.AddAsLeft(matMuls.Last());
                activations.Add(act);
                matMuls.Add(matmul);
            }
            softCrossEntropy.AddAsLeft(matMuls.Last());
            return matMuls;
        }
        private void CreateCalculatingGraph()
        {
            estimation = new SumNode();
            var weights = GetWeightsChain();
            output = new SoftCrossEntropyNode();
            observedInput = new ObservedOutputNode();
            estimation.AddAsLeft(output);
            output.AddAsRight(observedInput);
            var outputs = GetOutputs(output);
            input = new DataInputNode();
            for (int i = 0; i < outputs.Count; i++)
            {
                var gradientCollapseNode = new WeightsGradientCollapseNode();
                outputs[i].AddAsLeft(gradientCollapseNode);
                gradientCollapseNode.AddAsLeft(weights[i]);
            }
            outputs[0].AddAsRight(input);
            Subscribe(GetAllNodes(estimation));
            OnBatchSizeChanged.Invoke(this, 1);
            allNodes = GetAllNodes(estimation);
        }
        private Vector[] GetNormalize(Vector[] raw)
        {
            Vector[] result = new Vector[raw.Length];
            var inputLength = builder.Inputs.Length;
            for (int i = 0; i < raw.Length; i++)
            {
                var resVector = new Vector(new double[inputLength]);
                for (int j = 0; j < inputLength; j++)
                {
                    resVector[j] = builder.Inputs[j].Init(raw[i][j]);
                }
                result[i] = resVector;
            }
            for (int i = 0; i < result.Length; i++)
            {
                foreach (var value in result[i].Values)
                {
                    if(value  > 1 || value < 0)
                    {
                        throw new InvalidOperationException("Плохая нормализация");
                    }
                }
            }
            return result;
        }
        private Batch CalculateResult(Vector[] rawInputs)
        {
            input.Initiate(GetNormalize(rawInputs));
            var predicted = output.Predicted;
            OnResetData?.Invoke(false);
            return predicted;

        }
        private void CheckInputs(Vector[] inputs)
        {
            if (inputs is null)
            {
                throw new ArgumentNullException(nameof(inputs));
            }
            if (inputs.Length < 1)
            {
                throw new Exception();
            }
            var inputSize = inputs[0].Length;
            if (inputs.Any(x => x.Length != inputSize))
            {
                throw new Exception("Разные длины входных векторов");
            }
            if (inputSize != builder.Inputs.Length)
            {
                throw new Exception(
                    "Не совпадает размер вектора переданных данных " +
                    "и количество входных нейронов");
            }
        }
        private void BackProp()
        {
            foreach (var weightsNode in weightsNodes)
            {
                weightsNode.Train();
            }
        }
        private void CheckOutputs(Vector[] outputs)
        {
            if (outputs is null)
            {
                throw new ArgumentNullException(nameof(outputs));
            }
            if (outputs.Any(x => x.Length != builder.Outputs.Length))
            {
                throw new Exception("Не совпадает количество элементов в выходных векторах");
            }
        }


    }
}
