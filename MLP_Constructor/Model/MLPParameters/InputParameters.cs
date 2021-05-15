using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class InputParameters : PerceptronParameter
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string Name { get; set; }
        public InputParameters()
        {

        }
        public InputParameters(string name)
        {
            Name = name;
        }
        protected override bool CheckCorrect()
        {
            return MinValue < MaxValue && !(Name is null);
        }
    }
}
