using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.GraphParameters
{
    public class DoubleInput : Input<double>
    {
        public DoubleInput(double min, double max):base (min,max,new DoubleNormalizeProvider())
        {

        }
    }
}
