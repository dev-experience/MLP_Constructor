using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.Operations
{
    public class SigmoidOperation : Operation<double, double>
    {
        protected override Func<double, double> ConstructBack()
        {
            return x => Forward(x) * (1 - Forward(x));
        }

        protected override Func<double, double> ConstructForward()
        {
            return x => 1 / (1 + Math.Exp(-x));
        }
    }
}
