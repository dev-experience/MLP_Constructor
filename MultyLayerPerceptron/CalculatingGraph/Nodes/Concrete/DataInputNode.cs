using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class DataInputNode : InputNode
    {
        public void Initiate(params Vector[] inputs)
        {
            if (inputs is null)
            {
                throw new ArgumentNullException(nameof(inputs));
            }

            Init(new Batch(inputs));
        }
    }
}
