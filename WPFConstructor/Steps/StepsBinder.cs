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
        private ImmutableList<Step> steps = ImmutableList<Step>.Empty;
        public StepsBinder(IEnumerable<Step> currentSteps)
        {
            steps =steps.AddRange(currentSteps);
        }
        public IEnumerable<Step> GetSteps()
        {
            return steps;
        }
        public void Bind<T>() where T : Step, new()
        {
            var step = Step.GetInstance<T>();
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
