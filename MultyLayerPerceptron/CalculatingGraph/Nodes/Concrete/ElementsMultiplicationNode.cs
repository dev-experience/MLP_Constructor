using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class ElementsMultiplicationNode : Node
    {
        protected override Batch ComputeForwardResult(Batch leftResult, Batch rightResult)
        {
            return leftResult.ElementByElement(rightResult, (x, y) => x * y);
        }

        protected override Batch ComputeGradientByLeft(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            return rightResult.ElementByElement(inputGradientResult, (x, y) => x * y);
        }

        protected override Batch ComputeGradientByRight(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            return leftResult.ElementByElement(inputGradientResult, (x, y) => x * y);
        }
    }
}
