using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes.Concrete
{
    public class ObservedOutputNode : InputNode
    {
     

        public void Initiate(params Vector[] outputs)
        {
            if (outputs is null)
            {
                throw new ArgumentNullException(nameof(outputs));
            }

            if (outputs.Length !=BatchSize)
            {
            }
            Init(new Batch(outputs));
        }
    }
}
