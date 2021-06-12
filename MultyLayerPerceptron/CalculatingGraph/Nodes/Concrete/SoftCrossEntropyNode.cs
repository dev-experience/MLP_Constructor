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
            if (input[0].Values.All(x => x == 0))
            {

            }
            var input2 = input.ForEachMatrix(x => x.Clone() as Matrix);
            for (int i = 0; i < input2.Size; i++)
            {
                double min = input2[i].Values[0];
                for (int j = 1; j < input2[i].Values.Length; j++)
                {
                    double cur = input2[i].Values[j];
                    if (min > cur)
                    {
                        min = cur;
                    }
                }
                for (int j = 0; j < input2[i].Values.Length; j++)
                {
                    input2[i].Values[j] -= min;
                }
            }
            var res = new Batch(new Vector[input2.Size]);
            var sums = new Batch(new Scalar[input2.Size]);
            var exps = new Batch(new Vector[input2.Size]);
            for (int i = 0; i < exps.Size; i++)
            {
                exps[i] = input2[i]
                    .OperationForEachElement(x => Math.Exp(x));

                var expsVec = exps[i].AsVector;


                expsVec.Check();

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
            var outputLength = leftResult[0].Values.Length;
            for (int i = 0; i < result.Size; i++)
            {
                result[i] = Matrix.Scalar(0);
                for (int j = 0; j < outputLength; j++)
                {
                    var leftVec = softmaxOutput[i].AsVector;
                    var rightVec = rightResult[i].AsVector;
                    if (rightVec[j] == 0) continue;
                    result[i] -= Matrix.Scalar(Math.Log(leftVec[j]));
                }

            }



            for (int i = 0; i < result.Size; i++)
            {
                result[i].Check();
            }

            return result;
        }

        protected override Batch ComputeGradientByLeft(Batch inputGradientResult,
            Batch leftResult, Batch rightResult)
        {
            var predicted = Predicted;
            var observed = rightResult;
            /*  predicted = new Batch(
                 new Vector(new double[] { 0.15, 0.4, 0.46 }),
                 new Vector(new double[] { 0.04, 0.26, 0.7 }),
                 new Vector(new double[] { 0.04, 0.65, 0.31 }));
             BatchSize = 3;
             observed = new Batch(
                 new Vector(new double[] { 1, 0, 0 }),
                 new Vector(new double[] { 0, 1, 0 }),
                 new Vector(new double[] { 0, 0, 1 }));*/
            var outputLength = rightResult[0].AsVector.Length;
            Vector grads = new Vector(new double[outputLength]);
            for (int i = 0; i < outputLength; i++)
            {
                var currentObserved = observed[i].AsVector;


                double currentGrad = 0;
                for (int j = 0; j < BatchSize; j++)
                {
                    var currentPredicted = predicted[j].AsVector;
                    for (int k = 0; k < outputLength; k++)
                    {
                        var temp = currentObserved[k];
                        if (temp != 0)
                        {
                            currentGrad += temp * (currentPredicted[k] - DeltaFunction.GetResult(k, j));
                        }

                    }
                }
                grads[i] = currentGrad;
            }
            /*  var commonGrad = grads[0];
              for (int i = 1; i < grads.Size; i++)
              {
                  commonGrad += grads[i];
              }
            */
            //var result = grads.ForEachMatrix(inputGradientResult, (x, y) => x * y);
            var grad = new FakeBatch(BatchSize, grads);

            return grad;
        }

        internal double GetError()
        {
            int right = 0;
            var observedBatch = Right.Compute();
            var outputBatch = Left.Compute();
            double all = outputBatch.Size;
            Vector obsVec;
            Vector outVec;
            for (int i = 0; i < outputBatch.Size; i++)
            {
                obsVec = observedBatch[i].AsVector;
                outVec = outputBatch[i].AsVector;
                if (obsVec.MaxIndex() == outVec.MaxIndex()) right++;
            }
            return (all - right) / all;
        }


        protected override Batch ComputeGradientByRight(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            throw new NotImplementedException();
        }
    }
}
