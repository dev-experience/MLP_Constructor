using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFConstructor.Steps
{
    internal class StepsBinder
    {
        private ImmutableList<CustomStep> steps = ImmutableList<CustomStep>.Empty;
        private readonly StepByStepToken token;

        public StepsBinder(IEnumerable<CustomStep> currentSteps, StepByStepToken token)
        {
            steps =steps.AddRange(currentSteps);
            this.token = token;
        }
        public IEnumerable<CustomStep> GetSteps()
        {
            return steps;
        }
        public void Bind<T>() where T : CustomStep, new()
        {
            var step = CustomStep.GetInstance<T>(token);
            if (steps.Contains(step))
            {
                throw new InvalidOperationException($"Шаг уже существует в этапе. " +
                    $"Шаг: {step.Name}.");
            }
            if (step.ContainsDepthDependence(step))
            {
                throw new InvalidOperationException(
                    $"Обнаружена циклическая зависимость в [{step}]");
            }
            foreach (var item in step.Dependencies)
            {
                if (!steps.Contains(item))
                {
                    throw new InvalidOperationException(
                        $"Необходимо добавить все шаги, предшествующие данному. " +
                        $"Шаг {step.Name}. " +
                        $"Отсутствующая зависимость: {item.Name}.");
                }
            }
            steps = steps.Add(step);
        }
    }
}
