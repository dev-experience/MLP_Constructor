using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes
{
    public abstract class SingleInputNode : Node
    {
        protected sealed override Batch ComputeGradientByRight(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            return ComputeGradientByLeft(inputGradientResult,leftResult,rightResult);
        }
    }
}