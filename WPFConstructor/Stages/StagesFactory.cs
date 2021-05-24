using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFConstructor.Steps;

namespace WPFConstructor
{
    public abstract class StagesFactory
    {
        internal  IEnumerable<Stage> GetStages()
        {
            return Addressed(Token, ConstructStages().ToArray());
        }
       

        public StepByStepToken Token { get; private set; }
        public StagesFactory()
        {
            Token = StepByStepToken.New;
            CustomStep.RegisterToken(Token);
        }
        protected Stage CreateStage(string name, Action<StepsFactory> stepsSelector)
        {
            return new Stage(name, Token).AddSteps(stepsSelector);
        }
        private IEnumerable<Stage> Addressed(StepByStepToken token,  params Stage[] stages)
        {

            for (int i = 0; i < stages.Length; i++)
            {
                stages[i].SetToken(token);
                for (int j = 0; j < stages[i].Steps.Count; j++)
                {
                    stages[i].Steps[j].StepToken = token;
                    stages[i].Steps[j].Address = new StepAddress(j, i);
                    
                }
            }
            return stages;
        }
        protected abstract IEnumerable<Stage> ConstructStages();
        
    }
}
