using MultyLayerPerceptron;
using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using MultyLayerPerceptron.CalculatingGraph.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class PerceptronCreator
    {
        public int Id { get;  set; }
        public string Name { get; set; }
        public Perceptron Perceptron { get; private set; }
        public LearningRateParameter LearningRate { get; set; }
        public DataBaseParameters DataBase { get; set; }
        public List<HiddenLayerParameters> HiddenLayers { get; set; }
        public bool IsTrained { get; set; }
        public PerceptronCreator()
        {
            DataBase = new DataBaseParameters();
            LearningRate = new LearningRateParameter();
            HiddenLayers = new List<HiddenLayerParameters>();
            LearningRate.LearningRate = 0.0;
        }
        
        private bool CheckCorrectData()
        {
            if (HiddenLayers.Any(x => !x.IsCorrect)) return false;
            if (!DataBase.IsCorrect) return false;
            if (!LearningRate.IsCorrect) return false;

            return true;
        }
        public void ResetPerceptron()
        {
            Perceptron = null;
        }
        public bool TryCreate()
        {
            if (!CheckCorrectData())
            {
                Perceptron = null;
                return false;
            }

            var inputs = DataBase.GetConstructedInputs().ToArray();
            var temp = PerceptronBuilder
            .AddInput(inputs);
            foreach (var item in HiddenLayers.Select(x=>x.Size))
            {
                temp = temp.AddHidden(item);
            }
            var outputs = DataBase.GetConstructedOutputs().ToArray();
            Perceptron = temp
                .AddOutput(outputs)
                .SetLearningRate(0.01)
                .SetRegularizationRate(0);

            return true;
        }
        
        public override string ToString()
        {
            return $"[{Id}] \"{Name}\"";
        }
    }
}
