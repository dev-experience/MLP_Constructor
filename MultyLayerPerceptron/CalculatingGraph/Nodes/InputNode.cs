using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes
{
    public abstract class InputNode : Node
    {
        protected InputNode() : base(1)
        {
        }

        protected void Init(Batch inputs)
        {
            if (inputs.Size != BatchSize)
            {
                if (inputs is FakeBatch fakeInputs)
                {
                    fakeInputs.SetNewSize(BatchSize);
                }
                else
                {

                    throw new InvalidOperationException(
                        $"Данные не соответствуют размерам мини-пакета");
                }

            }
            result = inputs;
        }
        protected sealed override Batch ComputeGradientByLeft(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            throw new NotImplementedException();
        }

        protected sealed override Batch ComputeGradientByRight(Batch inputGradientResult, Batch leftResult, Batch rightResult)
        {
            throw new NotImplementedException();
        }

        protected sealed override Batch ComputeForwardResult(Batch leftResult, Batch rightResult)
        {
            return Compute();
        }
        public override bool IsCanResetResult => false;
    }
}
