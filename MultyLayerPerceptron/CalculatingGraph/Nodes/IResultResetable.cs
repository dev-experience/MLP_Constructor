using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes
{
   public interface IResultResetable
    {
        bool IsCanResetResult{ get; }
        void ResetResult(bool force);
    }
}
