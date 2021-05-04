using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFConstructor.Steps;

namespace WPFConstructor.Stages
{
    public  class ReadmeStageFactory : StagesFactory
    {
        protected  override IEnumerable<Stage> ConstructStages()
        {
            yield return CreateStage("Readme", x => x
            .AddByType<ReadmeStep1>()
            .AddByType<ReadmeStep2>()
            );
        }
    }
}
