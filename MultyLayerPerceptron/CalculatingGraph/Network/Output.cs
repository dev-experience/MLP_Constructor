using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.GraphParameters
{
    public class Output
    {
        public string Name { get; private set; }
        public Output(string name)
        {
            Name = name;
        }
    }
}
