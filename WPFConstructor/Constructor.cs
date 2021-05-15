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
        public event RoutedEventHandler OnOpenFile;
        public event RoutedEventHandler OnSaveFile;

        private Menu menu;
        private Panel content;
        private Panel footer;
        public bool ContentEnabled { get; set; }
        public List<Stage> Stages { get; set; }
        private StepAddress currentAddress;
        private readonly StepByStepToken token;

        public Constructor(StagesFactory stagesProvider, StepByStepToken token)
        {

            currentAddress = new StepAddress();
            Stages = new List<Stage>(stagesProvider.GetStages());
            this.token = token;
        }

        private Panel GetContent(StepAddress address)
        {
            if (ContentEnabled)
            {
                return FindStepByAddress(address).Content;
            }
            else
            {
                return new Grid();
            }
        }

        private void SaveMenuClick(object sender, RoutedEventArgs e)
        {
            OnSaveFile?.Invoke(sender, e);
        }

        private void OpenMenuClick(object sender, RoutedEventArgs e)
        {
            OnOpenFile?.Invoke(sender, e);
        }
        private MenuItem CreateFileMenu()
        {
            MenuItem file = new MenuItem() { Header = "Файл" };
            MenuItem open = new MenuItem() { Header = "Открыть" };
            open.Click += OpenMenuClick;
            file.Items.Add(open);
            if (token.DataContext is null) return file;

            MenuItem save = new MenuItem() { Header = "Сохранить" };
            save.Click += SaveMenuClick;
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
                    var incomlpeteStep = step.Dependencies.FirstOrDefault(x => !x.IsComplete);
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
            var targetAddress = ((sender as MenuItem).DataContext as CustomStep).Address;
            Relocate(targetAddress);
        }

        private Menu CreateMenu()
        {
            Menu _menu = new Menu();

            _menu.Items.Add(CreateFileMenu());
            if (ContentEnabled)
            {
                _menu.Items.Add(CreateNavigationMenu());
            }
            return _menu;
        }

        private void ForwardButtonClick(object sender, RoutedEventArgs e)
        {
            var nextStep = GetNextStep(currentAddress);
            Relocate(nextStep.Address);
        }
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            var prevStep = GetPreviewStep(currentAddress);
            Relocate(prevStep.Address);
        }
        private void EndButtonClick(object sender, RoutedEventArgs e)
        {
            var address = currentAddress;
            while (IsNextAvaliable(address))
            {
                address = GetNextStep(address).Address;
            }
            Relocate(address);
        }

        internal void Load()
        {
            if (ContentEnabled)
            {
                EndButtonClick(null, null);
            }
            else
            {
                UpdateMenu();
            }
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
            if (!ContentEnabled)
            {
                return footerGrid;
            }
            var isNextAvaliable = IsNextAvaliable(currentAddress);
            footerGrid.AddColumns(GridUnitType.Star, 0.5, 1, 1, 0.5);
            var column = 0;
            footerGrid.Children.Add(CreateButton(
               "В начало", 0, column++, (plug1, plug2) => Relocate(), IsPreviewAvaliable(currentAddress)));
            footerGrid.Children.Add(CreateButton(
                "Назад", 0, column++, BackButtonClick, IsPreviewAvaliable(currentAddress)));
            footerGrid.Children.Add(CreateButton(
                "Вперед", 0, column++, ForwardButtonClick, isNextAvaliable));
            footerGrid.Children.Add(CreateButton(
                "В конец", 0, column++, EndButtonClick, isNextAvaliable));
            return footerGrid;

        }



        private CustomStep FindStepByAddress(StepAddress address)
        {
            return Stages
                .SelectMany(x => x.Steps)
                .FirstOrDefault(x => x.Address == address);
        }
        private CustomStep GetPreviewStep(StepAddress address)
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
        private CustomStep GetNextStep(StepAddress address)
        {
            return FindStepByAddress(new StepAddress(address.Step + 1, address.Stage)) ??
                FindStepByAddress(new StepAddress(stage: address.Stage + 1));
        }
        public void Update()
        {
            UpdateMenu();
            UpdateContent();
            UpdateFooter();
        }
        public void Recheck()
        {
            UpdateMenu();
            UpdateFooter();
        }
        private void UpdateContent()
        {
            content = GetContent(currentAddress);
            ContentChanged?.Invoke(this, content);
        }
        private void UpdateFooter()
        {
            footer = CreateFooter(currentAddress);
            FooterChanged?.Invoke(this, footer);
        }

        private void UpdateMenu()
        {
            menu = CreateMenu();
            MenuChanged?.Invoke(this, menu);
        }
        public void Relocate(StepAddress address = default)
        {
            foreach (var item in Stages.SelectMany(x => x.Steps))
            {
                item.Reset();
            }
            currentAddress = address;
            Update();
        }
    }
}
