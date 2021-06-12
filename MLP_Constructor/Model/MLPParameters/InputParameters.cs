using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class InputParameters : PerceptronParameter,IDbColumn
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
        public void SetMinMaxValue(double value)
        {
            if (value >= MaxValue)
            {
                MaxValue = value;
            }
            else if(value<=MinValue)
            {
                MinValue = value;
            }
        }
        public string DbType => "DECIMAL(18,6)";
        public string DbName => $"[{Name}]";

    }
}
