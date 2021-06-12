using MLP_Constructor.Model.MLPParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.Supported
{
    public class OpenFileEventArgs
    {
        public OpenFileEventArgs(PerceptronCreator creator)
        {
            Creator = creator;
        }

        public PerceptronCreator Creator { get; }
    }
}
