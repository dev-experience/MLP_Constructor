using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.Operations
{
    public static class OperationsFactory
    {
        public static LeakyReLUOperation LeakyReLU(double parameter)
        {
            return new LeakyReLUOperation(parameter);
        }
        public static SigmoidOperation Sigmoid()
        {
            return new SigmoidOperation();
        }
    }

}
