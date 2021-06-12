using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class WeightsGradientCollapseNode : SingleInputNode
    {

        protected override Batch ComputeForwardResult(Batch leftResult, Batch rightResult)
        {
            return new FakeBatch(BatchSize, leftResult[0]);
        }

        protected override Batch ComputeGradientByLeft(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            
            var result = new FakeBatch(inputGradientResult.Size,inputGradientResult[0]);
            for (int i = 1; i < result.Size; i++)
            {
              //  result[i] += inputGradientResult[i];
            }
            return result;
        }
    }
}
