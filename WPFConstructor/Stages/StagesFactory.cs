using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFConstructor
{
    public abstract class StagesFactory
    {
        public  IEnumerable<Stage> GetStages(StepByStepToken token)
        {
            return Addressed(token, ConstructStages().ToArray());
        }
        private IEnumerable<Stage> Addressed(StepByStepToken token,  params Stage[] stages)
        {

            for (int i = 0; i < stages.Length; i++)
            {
                for (int j = 0; j < stages[i].Steps.Count; j++)
                {
                    stages[i].Steps[j].Address = new StepAddress(j, i);
                }
            }
            return stages;
        }
        protected abstract IEnumerable<Stage> ConstructStages();
        
    }
}
