using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class ActivationNode : SingleInputNode
    {

        public Operation<double, double> ActivationFunc { get; }
        public ActivationNode(Operation<double, double> activationFunc)
        {
            ActivationFunc = activationFunc;
        }


        protected override Batch ComputeForwardResult(Batch leftResult,
            Batch rightResult)
        {
            var result =  leftResult.ForEachMatrix(x=>x
                .OperationForEachElement(z => ActivationFunc
                .Forward(z)));
            return result;
        }

        protected override Batch ComputeGradientByLeft(Batch inputGradientResult,
            Batch leftResult, Batch rightResult)
        {
            return leftResult.ForEachMatrix(x => x
                  .OperationForEachElement(z => ActivationFunc
                  .Back(z)))
                  .ElementByElement(inputGradientResult,(x,y)=>x*y);
        
        }
    }
}
