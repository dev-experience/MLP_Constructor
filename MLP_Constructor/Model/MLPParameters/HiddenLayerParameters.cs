using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class HiddenLayerParameters : PerceptronParameter
    {
        
        public int Size { get; set; }
        public HiddenLayerParameters(int size)
        {
            Size = size;
        }
        protected override bool CheckCorrect()
        {
            return Size > 0;
                 
        }
    }
}
