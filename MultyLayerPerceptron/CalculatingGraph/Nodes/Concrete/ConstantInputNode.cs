using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class ConstantInputNode : InputNode
    {

        public ConstantInputNode(double value):base()
        {
            Initiate(value);
        }
        public void Initiate(double value)
        {

            Init(new FakeBatch(BatchSize, new Scalar(value)));
        }
    }
}
