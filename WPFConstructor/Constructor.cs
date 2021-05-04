using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFConstructor;

namespace WPFConstructor
{
    internal class Constructor
    {
        public event UIElementChangedEventHandler MenuChanged;
        public event UIElementChangedEventHandler ContentChanged;
        public event UIElementChangedEventHandler FooterChanged;
        private Menu menu;
        private Panel content;
        private Panel footer;
        public List<Stage> Stages { get; set; }
        public StepAddress CurrentAddress { get; private set; }
        public Constructor(StagesFactory stagesProvider, StepByStepToken token)
        {
            
            CurrentAddress = new StepAddress();
            Stages = new List<Stage>(stagesProvider.GetStages(token));
        }

        private Panel GetContent(StepAddress address)
        {
            return Stages[CurrentAddress.Stage].GetContent(CurrentAddress.Step);
        }
        private MenuItem CreateFileMenu()
        {
            MenuItem file = new MenuItem() { Header = "Файл" };
            MenuItem open = new MenuItem() { Header = "Открыть" };
            MenuItem save = new MenuItem() { Header = "Сохранить" };
            file.Items.Add(open);
            file.Items.Add(save);
            return file;
        }
        private MenuItem CreateNavigationMenu()
        {
            MenuItem nav = new MenuItem { Header = "Навигация" };
            foreach (var stage in Stages)
            {
                var stageItem = new MenuItem { Header = stage };
                foreach (var step in stage.Steps)
                {
                    var isAvaliable = step.IsDependenciesComplete;
                    var incomlpeteStep = step.Dependencies.FirstOrDefault(x=>!x.IsComplete);
                    StringBuilder str = new StringBuilder();
                    str.Append(step);
                    if (incomlpeteStep != null)
                    {
                        str.Append($" (Необходимо: {incomlpeteStep.Address})");
                    }
                    
                    var stepItem = new MenuItem { DataContext = step, Header = str };
                    stepItem.IsEnabled = incomlpeteStep == null;
                    stepItem.Click += NavigationMenuClick;
                    stageItem.Items.Add(stepItem);
                }
                nav.Items.Add(stageItem);
            }
            return nav;
        }

        private void NavigationMenuClick(object sender, RoutedEventArgs e)
        {
            var targetAddress = ((sender as MenuItem).DataContext as Step).Address;
            Relocate(targetAddress);
        }

        private Menu CreateMenu()
        {
            Menu _menu = new Menu();

            _menu.Items.Add(CreateFileMenu());
            _menu.Items.Add(CreateNavigationMenu());
            return _menu;
        }

        private void ForwardButtonClick(object sender, RoutedEventArgs e)
        {
            var nextStep = GetNextStep(CurrentAddress);
            Relocate(nextStep.Address);
        }
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            var prevStep = GetPreviewStep(CurrentAddress);
            Relocate(prevStep.Address);
        }
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            var currentStep = FindStepByAddress(CurrentAddress);
            currentStep.Reset();
            ContentChanged?.Invoke(this, currentStep.Content);
        }
        private void EndButtonClick(object sender, RoutedEventArgs e)
        {
            var address = CurrentAddress;
            while (IsNextAvaliable(address))
            {
                address = GetNextStep(address).Address;
            }
            Relocate(address);
        }
        private Button CreateButton(object content, int row, int column, RoutedEventHandler click, bool isEnabled)
        {
            Button btn = new Button();
            btn.Content = content;
            btn.Click += click;
            btn.IsEnabled = isEnabled;
            Grid.SetRow(btn, row);
            Grid.SetColumn(btn, column);
            return btn;
        }

        private Panel CreateFooter(StepAddress address = default)
        {
            Grid footerGrid = new Grid();
            var isNextAvaliable = IsNextAvaliable(CurrentAddress);
            footerGrid.AddColumns(GridUnitType.Star, 0.5,1, 0.5, 1, 0.5);
            var column = 0;
            footerGrid.Children.Add(CreateButton(
               "В начало", 0, column++, (plug1, plug2) => Relocate(), IsPreviewAvaliable(CurrentAddress)));
            footerGrid.Children.Add(CreateButton(
                "Назад", 0, column++, BackButtonClick, IsPreviewAvaliable(CurrentAddress)));
            footerGrid.Children.Add(CreateButton(
                "Сбросить", 0, column++, UpdateButtonClick, true));
            footerGrid.Children.Add(CreateButton(
                "Вперед", 0, column++, ForwardButtonClick, isNextAvaliable));
            footerGrid.Children.Add(CreateButton(
                "В конец", 0, column++, EndButtonClick, isNextAvaliable));
            return footerGrid;

        }



        private Step FindStepByAddress(StepAddress address)
        {
            return Stages
                .SelectMany(x => x.Steps)
                .FirstOrDefault(x => x.Address == address);
        }
        private Step GetPreviewStep(StepAddress address)
        {
            if (address == new StepAddress(0, 0))
            {
                return null;
            }
            if (address.Step == 0)
            {
                var previewStage = address.Stage - 1;
                var maxStep = Stages
                    .SelectMany(x => x.Steps)
                    .Where(x => x.Address.Stage == previewStage)
                    .Max(x => x.Address.Step);
                address = new StepAddress(previewStage, maxStep);
            }
            else
            {
                address = new StepAddress(address.Step - 1, address.Stage);
            }
            return FindStepByAddress(address);
        }
        private bool IsPreviewAvaliable(StepAddress address)
        {
            var step = GetPreviewStep(address);
            return step != null && step.IsDependenciesComplete;
        }

        private bool IsNextAvaliable(StepAddress address)
        {
            var next = GetNextStep(address);
            return next != null && next.IsDependenciesComplete;
        }
        private Step GetNextStep(StepAddress address)
        {
            return FindStepByAddress(new StepAddress(address.Step + 1, address.Stage)) ??
                FindStepByAddress(new StepAddress(stage: address.Stage + 1));
        }
        public void Relocate(StepAddress address = default)
        {
            CurrentAddress = address;
            footer = CreateFooter(CurrentAddress);
            menu = CreateMenu();
            content = GetContent(CurrentAddress);

            MenuChanged?.Invoke(this, menu);
            ContentChanged?.Invoke(this, content);
            FooterChanged?.Invoke(this, footer);
        }
    }
}
