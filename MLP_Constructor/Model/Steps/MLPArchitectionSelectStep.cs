using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFConstructor;

namespace MLP_Constructor.Model.Steps
{
    public class MLPArchitectionSelectStep : CustomStep
    {
        public override string Name => "Выбор конфигурации нейросети";

        protected override IEnumerable<CustomStep> CreateDependencies()
        {
            yield return GetInstance<InputOutputSelectionStep>(StepToken);
        }
        protected override Panel CreateContent()
        {
            return new Grid();
        }
    }
}
