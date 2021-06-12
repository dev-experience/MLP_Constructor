using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron
{
    public static class DeltaFunction
    {
        public static int GetResult(int i, int j)
        {
            return i == j ? 1 : 0;
        }
    }
}
