using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron
{
    public interface INormalizeProvider<T>
    {
        double Normalize(T inputValue, T minValue, T maxValue);
    }
}