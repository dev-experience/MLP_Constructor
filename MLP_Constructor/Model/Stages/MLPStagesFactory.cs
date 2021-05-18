using MLP_Constructor.Model.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFConstructor;

namespace MLP_Constructor.Model.Stages
{
    public class MLPStagesFactory : StagesFactory
    {
        protected override IEnumerable<Stage> ConstructStages()
        {
            yield return CreateStage("Конфигурация нейросети", x => x
             .AddByType<DataSourceSelectionStep>()
             .AddByType<InputOutputSelectionStep>()
             .AddByType<DataPreprocessingStep>()
             .AddByType<MLPArchitectionSelectStep>()
             .AddByType<TrainPerceptronStep>()
            );
        }
    }
}
