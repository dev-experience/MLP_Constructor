using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    class SumNode : Node
    {
   

        protected override Batch ComputeForwardResult(Batch leftResult, Batch rightResult)
        {
            
            return leftResult.Sum(rightResult);
        }
        protected override Batch ComputeGradientByLeft(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            int rows = leftResult[0].Rows;
            int cols = leftResult[0].Columns;
            return new FakeBatch(BatchSize,
                new Matrix(rows, cols, new double[] { }).Fill(1));
        }

        protected override Batch ComputeGradientByRight(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            int rows = inputGradientResult[0].Rows;
            int cols = inputGradientResult[0].Columns;
            return new FakeBatch(BatchSize,
                new Matrix(rows, cols, null).Fill(1));
        }
    }
}
