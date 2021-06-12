using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class MatrixMultiplicationNode : Node
    {
        protected override Batch ComputeForwardResult(Batch leftResult, Batch rightResult)
        {
            var result = leftResult * rightResult;
            if (result.Size < 500)
            {

            }
            return result;
        }

        protected override Batch ComputeGradientByLeft(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            var res = inputGradientResult * rightResult.ForEachMatrix(x => x.Transposed());
            return res;
        }

        protected override Batch ComputeGradientByRight(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            return leftResult.ForEachMatrix(x => x.Transposed()) * inputGradientResult;
        }
    }
}
