using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class LearningRateParameter : PerceptronParameter
    {
        public double LearningRate { get; set; }
        protected override bool CheckCorrect()
        {
            return LearningRate > 0;
        }
    }
}
