using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class SoftCrossEntropyNode : Node
    {
        public Batch Predicted => SoftMax(Left.Compute());
        private Batch SoftMax(Batch input)
        {
            var res = new Batch(new Vector[input.Size]);
            var sums = new Batch(new Scalar[input.Size]);
            var exps = new Batch(new Vector[input.Size]);
            for (int i = 0; i < exps.Size; i++)
            {
                exps[i] = input[i]
                    .OperationForEachElement(x => Math.Exp(x));
                var expsVec = exps[i].AsVector;
                var sum = 0.0;
                for (int j = 0; j < expsVec.Length; j++)
                {
                    sum += expsVec[j];
                }
                if (sum == 0)
                {
                    sum = expsVec.Length;
                }
                res[i] = expsVec.OperationForEachElement(x => x / sum);

            }
            return res;
        }
        protected override Batch ComputeForwardResult(Batch leftResult,
            Batch rightResult)
        {
            var result = new Batch(new Matrix[leftResult.Size]);
            var softmaxOutput = SoftMax(leftResult);
            var outputLength = leftResult[0].Columns;
            for (int i = 0; i < result.Size; i++)
            {
                result[i] = Matrix.Scalar(0);
                for (int j = 0; j < outputLength; j++)
                {
                    var leftVec = softmaxOutput[i].AsVector;
                    var rightVec = rightResult[i].AsVector;
                    result[i] -= Matrix.Scalar(Math.Log(leftVec[j] * rightVec[j]));
                }

            }
            return result;
        }

        protected override Batch ComputeGradientByLeft(Batch inputGradientResult,
            Batch leftResult, Batch rightResult)
        {
            var outputLength = rightResult[0].AsVector.Length;
            Batch grads = new Batch(new Matrix[BatchSize]);

            for (int i = 0; i < BatchSize; i++)
            {
                var rightVec = rightResult[i].AsVector;
                var leftVec = leftResult[i].AsVector;
                Vector currentGrad = new Vector(new double[outputLength]);
                for (int j = 0; j < outputLength; j++)
                {
                    for (int k = 0; k < outputLength; k++)
                    {
                        if (rightVec[k] != 0)
                        {
                            currentGrad[j] += rightVec[k] * (leftVec[k] - DeltaFunction.GetResult(j, k));
                        }

                    }
                }
                grads[i] = currentGrad;
            }
            var result = grads.ForEachMatrix(inputGradientResult, (x, y) => x * y);
            return result;
        }

        protected override Batch ComputeGradientByRight(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            throw new NotImplementedException();
        }
    }
}
