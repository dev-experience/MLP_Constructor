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
    public abstract class CustomStep
    {
        private Panel content;
        private IEnumerable<CustomStep> dependencies = new List<CustomStep>();
        private static Dictionary<StepByStepToken, List<CustomStep>> instancies =
            new Dictionary<StepByStepToken, List<CustomStep>>();
        private bool isInit=false;
        private bool CheckDependenciesComplete()
        {
            if (dependencies.Count() > 0)
            {
                return dependencies.All(x => x.IsComplete);
            }
            return true;
        }
        protected abstract Panel CreateContent();
        protected virtual IEnumerable<CustomStep> CreateDependencies()
        {
            return new List<CustomStep>();
        }
        protected virtual bool CheckComplete()
        {
            return true;
        }
        public bool IsDependenciesComplete => CheckDependenciesComplete();
        public StepAddress Address { get; set; }
        public StepByStepToken StepToken { get; set; }
        public abstract string Name { get; }

        public Panel Content
        {
            get
            {
                if(content is null)
                {
                    Reset();
                }
                return content;
            }
        }
        public bool IsComplete => isInit && CheckDependenciesComplete() && CheckComplete();
        public IEnumerable<CustomStep> Dependencies { get => dependencies; }
        public bool ContainsDepthDependence(CustomStep step)
        {
            if (dependencies.Count() == 0) return false;
            if (dependencies.Contains(step)) return true;
            return dependencies.Any(x => x.ContainsDepthDependence(step));
        }
        public void Reset()
        {
            if (StepToken.DataContext is null)
            {
                return;
            }
            content = CreateContent();
            isInit = true;
            UpdateContent();
            Check();
        }
        protected virtual void UpdateContent()
        {
            return;
        }
        private bool isComplete;
        protected void Check()
        {
            if (isInit && isComplete != CheckComplete())
            {
                isComplete = CheckComplete();
                
            StepToken.InteractWithConstructor(x => x.Recheck());
            }
        }
        protected void Update(bool force = false)
        {
            if (force) UpdateContent();
            StepToken.InteractWithConstructor(x => x.Update());
        }
        public static CustomStep GetInstance<T>(StepByStepToken token) where T : CustomStep, new()
        {
            if (!instancies.ContainsKey(token))
            {
                instancies.Add(token, new List<CustomStep>());
            }
            var instance = instancies[token].FirstOrDefault(x => x.GetType() == typeof(T));
            if (instance == null)
            {
                instance = new T();
                instance.StepToken = token;
            }
            instancies[token].Remove(instance);
            instancies[token].Add(instance);
            return instance;
        }
        public override string ToString()
        {
            return $"Шаг {Address.Step + 1}. {Name}.";
        }
        public CustomStep()
        {
            dependencies = CreateDependencies();

        }
    }
}
