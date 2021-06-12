using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public abstract class PerceptronParameter
    {
        public bool IsCorrect => CheckCorrect();

        protected abstract bool CheckCorrect();

    }
}
