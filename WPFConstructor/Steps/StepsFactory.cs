using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFConstructor.Steps
{
    public class StepsFactory
    {

        private StepsBinder stepsBinder;
        internal StepsFactory(StepsBinder stepsBinder)
        {
            this.stepsBinder = stepsBinder;
        }

       

        public StepsFactory AddByType<T>() where T : CustomStep ,new()
        {
            stepsBinder.Bind<T>();
            return this;
        }

    }
}
