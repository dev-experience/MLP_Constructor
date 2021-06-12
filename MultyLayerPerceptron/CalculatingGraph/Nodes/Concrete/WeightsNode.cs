using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class WeightsNode : InputNode, ITrainable
    {
        public WeightsNode(double learningRate)
        {
            LearningRate = learningRate;
        }

        public double LearningRate { get; set; }
        public void Train()
        {
            var ownGrad = GetOwnGradient();
            var addiction = ownGrad * LearningRate;
            
            if (addiction is FakeBatch fake)
            {
                fake[0].Check();
            }
            else
            {

                for (int i = 0; i < addiction.Size; i++)
                {
                    addiction[i].Check();
                }
            }
            result = new FakeBatch(BatchSize, result[0] - addiction[0]);
        }
        public void Initiate(Matrix weights,
            WeightDirectionType directionType = WeightDirectionType.FromTo)
        {

            Init(new FakeBatch(BatchSize,
                directionType == WeightDirectionType.ToFrom ? weights : weights.Transposed()));
        }
    }
}
