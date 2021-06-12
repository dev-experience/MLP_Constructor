using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron
{
    public interface ISummable<T>
    {
        T Sum(T second);
    }
}
