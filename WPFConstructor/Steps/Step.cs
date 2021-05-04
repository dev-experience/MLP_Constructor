using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFConstructor
{
    public abstract class Step
    {
        private Panel content;
        private IEnumerable<Step> dependencies = new List<Step>();
        private Func<bool> completeCondition;
        private static Dictionary<StepByStepToken, List<Step>> instancies =
            new Dictionary<StepByStepToken, List<Step>>();
        private bool CheckDependenciesComplete()
        {
            if (dependencies.Count() > 0)
            {
                return dependencies.All(x => x.IsComplete);
            }
            return true;
        }
        protected abstract Panel CreateContent();
        protected virtual IEnumerable<Step> CreateDependencies()
        {
            return new List<Step>();
        }
        protected virtual Func<bool> CreateCompleteCondition()
        {
            return () => true;
        }
        public bool IsDependenciesComplete => CheckDependenciesComplete();
        public StepAddress Address { get; set; }
        public StepByStepToken StepToken { get; set; }
        public abstract string Name { get; }
        public Panel Content { get => content; set => content = value; }
        public bool IsComplete => CheckDependenciesComplete() && completeCondition();
        public IEnumerable<Step> Dependencies { get => dependencies; }
        public event EventHandler OnChangeData;
        public bool ContainsDepthDependence(Step step)
        {
            if (dependencies.Count() == 0) return false;
            if (dependencies.Contains(step)) return true;
            return dependencies.Any(x => x.ContainsDepthDependence(step));
        }
        public void Reset()
        {
            content = CreateContent();
        }
        public static void Reset<T>(StepByStepToken token) where T : Step, new()
        {

            if (!instancies.ContainsKey(token))
            {
                throw new ArgumentException($"Токен {token} не найден в словаре ");
            }
            instancies[token].First(x => x.GetType() == typeof(T)).Reset();
        }
        public static Step GetInstance<T>(StepByStepToken token) where T : Step, new()
        {
            if (!instancies.ContainsKey(token))
            {
                instancies.Add(token, new List<Step>());
            }
            var instance = instancies[token].FirstOrDefault(x => x.GetType() == typeof(T));
            if (instance == null)
            {
                instance = new T();
                instance.StepToken = token;
                instance.Reset();
            }
            instancies[token].Remove(instance);
            instancies[token].Add(instance);
            return instance;
        }
        public override string ToString()
        {
            return $"Шаг {Address.Step + 1}. {Name}.";
        }
        public Step()
        {
            dependencies = CreateDependencies();
           
            completeCondition = CreateCompleteCondition();
        }
    }
}
