using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using MultyLayerPerceptron.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Network
{
    public class PerceptronBuilder
    {
        public Input[] Inputs { get; set; }
        public List<Matrix> Weights { get; set; }
        public Output[] Outputs { get; set; }
        public Operation<double,double> ActivationFunc { get; set; }
        private double learningRate=1;
        public double LearningRate
        {
            get { return learningRate; }
            set { learningRate= value; }
        }
        private double regularizationRate=1;
        public double RegularizationRate
        {
            get { return regularizationRate; }
            set { regularizationRate = value; }
        }
        private PerceptronBuilder()
        {
            ActivationFunc = OperationsFactory.LeakyReLU(0.01);
            Weights = new List<Matrix>();
        }
        public static HiddenLayer AddInput(params Input[] inputs)
        {
            var perceptron = new PerceptronBuilder();
            perceptron.Inputs = inputs;
            return new HiddenLayer(perceptron);
        }

    }

}
