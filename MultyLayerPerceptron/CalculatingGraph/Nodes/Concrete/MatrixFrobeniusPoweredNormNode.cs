using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class MatrixFrobeniusPoweredNormNode : SingleInputNode
    {
        protected override Batch ComputeForwardResult(Batch leftResult, Batch rightResult)
        {

            var a = leftResult.ForEachMatrix(leftResult, (x, y) => y.PoweredNormF());
            return a;
        }

        protected override Batch ComputeGradientByLeft(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            var simpleGradientPart = leftResult.ForEachMatrix(x => x * 2.0);
            return simpleGradientPart.ElementByElement(inputGradientResult, (x, y) => x * y);
        }
    }
}
