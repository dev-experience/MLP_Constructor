using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron
{
    public class Scalar : Matrix
    {
        public Scalar(double value) : base(1,1, new double[] { value })
        {
        }

    }
}
