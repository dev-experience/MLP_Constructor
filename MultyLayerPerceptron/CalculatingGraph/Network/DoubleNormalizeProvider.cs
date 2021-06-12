using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph
{
    public class DoubleNormalizeProvider : INormalizeProvider<double>
    {
        public double Normalize(double inputValue, double minValue, double maxValue)
        {
            return (inputValue - minValue) / (maxValue-minValue);
        }
    }
}
