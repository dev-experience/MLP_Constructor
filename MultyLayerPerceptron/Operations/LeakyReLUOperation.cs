using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.Operations
{
    public class LeakyReLUOperation : Operation<double, double>
    {
        private readonly double parameter;

        public LeakyReLUOperation(double parameter)
        {
            this.parameter = parameter;
        }
        

        protected override Func<double, double> ConstructForward()
        {
            return x => x > 0 ? x : parameter * x;
        }
        protected override Func<double, double> ConstructBack()
        {
            return x => x >= 0 ? 1 : parameter;
        }
    }
}
