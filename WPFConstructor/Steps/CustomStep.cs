using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFConstructor.Steps;

namespace WPFConstructor
{
    public abstract class CustomStep
    {
        private Panel content;
        public static void RegisterToken(StepByStepToken token)
        {
            instancies.Add(token, new List<CustomStep>());
        }
        private IEnumerable<CustomStep> dependencies = new List<CustomStep>();
        private static Dictionary<StepByStepToken, List<CustomStep>> instancies =
            new Dictionary<StepByStepToken, List<CustomStep>>();
        private List<CustomStep> inverseDependencies = new List<CustomStep>();
        public bool IsComplete { get; private set; }
        public bool IsDependenciesComplete { get; private set; }

        protected virtual bool CheckComplete() { return true; }
        protected abstract Panel CreateContent();
        protected virtual IEnumerable<CustomStep> CreateDependencies()
        {
            return new List<CustomStep>();
        }
        protected virtual void UpdateContent()
        {

        }
        private void CheckDependencies()
        {
            IsDependenciesComplete = dependencies.All(x => x.IsComplete);
        }
        protected void Check()
        {
            if (IsComplete != CheckComplete())
            {
                IsComplete = !IsComplete;
                StepChanged?.Invoke(this, new ChangedStepEventArgs(IsComplete));
            }
        }
        public event ChangedStepEventHandler StepChanged;
        private void OnDependencyChanged(object sender, ChangedStepEventArgs e)
        {
            if (!e.Complete)
            {
                IsDependenciesComplete = false;
            }
            else if (!IsDependenciesComplete)
            {
                CheckDependencies();
            }
        }
        public abstract string Name { get; }
        public StepAddress Address { get; set; }
        public StepByStepToken StepToken { get; set; }
        
        public void Reload()
        {
            if (StepToken.DataContext is null) return;
            if (content is null) content = CreateContent();
            UpdateContent();
            Check();
        }
        public Panel Content
        {
            get
            {
                if (content is null)
                {
                    content = CreateContent();
                }
                return content;
            }
        }
        public IEnumerable<CustomStep> Dependencies { get => dependencies; }
        public bool ContainsDepthDependence(CustomStep step)
        {
            if (dependencies.Count() == 0) return false;
            if (dependencies.Contains(step)) return true;
            return dependencies.Any(x => x.ContainsDepthDependence(step));
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
                instance.Init(token);
            }
            instancies[token].Remove(instance);
            instancies[token].Add(instance);
            return instance;
        }
        public override string ToString()
        {
            return $"Шаг {Address.Step + 1}. {Name}.";
        }
        private void Init(StepByStepToken token)
        {
            StepToken = token;
            foreach (var item in CreateDependencies())
            {
                item.StepChanged += OnDependencyChanged;
            }
            CheckDependencies();
            
            
        }
        public CustomStep()
        {
            dependencies = CreateDependencies();
     

        }
    }
}
