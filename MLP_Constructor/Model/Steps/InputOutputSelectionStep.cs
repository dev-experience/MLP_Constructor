using MLP_Constructor.Model.MLPParameters;
using MLP_Constructor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFConstructor;

namespace MLP_Constructor.Model.Steps
{
    public class InputOutputSelectionStep : CustomStep
    {
        public override string Name => "Выбор входных данных и классифицируемого параметра";
        protected override IEnumerable<CustomStep> CreateDependencies()
        {
            yield return GetInstance<DataSourceSelectionStep>(StepToken);
        }
        private PerceptronCreator creator;
        private IEnumerable<string> columnNames;
        private ScrollViewer inputsList;
        private ScrollViewer outputsList;
        private ScrollViewer outputVariantsList;
        private Button deleteButton;
        private Button unionButton;
        private Button showButton;
        private Grid btnPanel;
        private TextBlock label1;
        private TextBlock label2;
        private TextBlock label3;
        private Grid content;
        private RadioButton checkedOutput;
        private List<CheckBox> inputsChecked = new List<CheckBox>();
        private List<CheckBox> outputVariantsChecked = new List<CheckBox>();
        protected override bool CheckComplete()
        {
            var cond1 = inputsChecked.Count > 0;
            var cond2 = !(checkedOutput is null);
            bool cond3 = false;
            if (outputVariantsList.Content is null)
            {
                cond3 = false;
            }
            else
            {
                cond3 = (outputVariantsList.Content as StackPanel).Children.Count > 1;
            }
            return cond1 && cond2 && cond3;
        }
        protected override void UpdateContent()
        {
            columnNames = creator.DataBase.GetColumnNames();
            inputsList.Content = CreateInputs();
            outputsList.Content = CreateOutputs();
            UpdateOutputVariants();

        }
        private void UpdateOutputVariants()
        {
            outputVariantsChecked = new List<CheckBox>();
            outputVariantsList.Content = CreateOutputVariants();
            CheckVariantManipulateButtons();
            Check();

        }
        private StackPanel CreateInputs()
        {
            StackPanel inputs = new StackPanel();
            inputsChecked = new List<CheckBox>();
            foreach (var name in columnNames)
            {
                CheckBox input = new CheckBox();
                input.Content = name;
                input.Checked += OnSelectInput;
                input.Unchecked += OnDeselectInput;
                if (creator.DataBase.Inputs.Any(x => x.Name.Equals(name)))
                {
                    input.IsChecked = true;
                }
                inputs.Children.Add(input);
            }
            return inputs;

        }



        private StackPanel CreateOutputs()
        {
            StackPanel outputs = new StackPanel();
            outputs.CanHorizontallyScroll = true;
            foreach (var name in columnNames)
            {
                RadioButton output = new RadioButton();
                output.GroupName = "outputs";
                output.Content = name;
                output.Checked += OnSelectOutput;
                if (name.Equals(creator.DataBase.OutputClassName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    output.IsChecked = true;
                }
                outputs.Children.Add(output);
            }
            return outputs;
        }

        private StackPanel CreateOutputVariants()
        {
            StackPanel variants = new StackPanel();
            int limitCounter = 0;
            foreach (var item in creator.DataBase.Outputs)
            {
                if (++limitCounter > 100)
                {
                    StackPanel error = new StackPanel();
                    TextBlock errorText = new TextBlock();
                    errorText.Text = "Вариантов выхода классификатора должно быть меньше сотни";
                    error.Children.Add(errorText);
                    return error;
                }

                var variant = new CheckBox();
                variant.Content = item.Name;
                variant.ToolTip = item.CreateToolTip();
                variant.Checked += OnOutputVariantChecked;
                variant.Unchecked += OnOutputVariantUnchecked;
                variants.Children.Add(variant);
            }
            if (limitCounter == 0)
            {
                StackPanel error = new StackPanel();
                TextBlock errorText = new TextBlock();
                errorText.Text = showButton.IsEnabled ? "Не загружено" : "Пусто:(";
                error.Children.Add(errorText);
                return error;
            }

            return variants;
        }
        private void CheckVariantManipulateButtons()
        {
            var count = outputVariantsChecked.Count;
            deleteButton.IsEnabled = count > 0;
            deleteButton.Content = "Удалить";
            unionButton.IsEnabled = count > 0;
            unionButton.Content = count > 1 ? "Объединить" : "Переименовать";
            if (outputVariantsList.Content is null)
            {
                return;
            }
            var allsCount = (outputVariantsList.Content as StackPanel).Children.Count;
            var uncheckedCount = allsCount - count;
            if (uncheckedCount == 0)
            {
                unionButton.IsEnabled = false;
                unionButton.Content = "Выходов должно быть как минимум 2";
            }
            if (uncheckedCount < 2 || allsCount <= 2)
            {
                deleteButton.IsEnabled = false;
                deleteButton.Content = "Выходов должно быть как минимум 2";
            }
        }
        private void OnOutputVariantUnchecked(object sender, RoutedEventArgs e)
        {
            outputVariantsChecked.Remove(sender as CheckBox);
            CheckVariantManipulateButtons();
        }

        private void OnOutputVariantChecked(object sender, RoutedEventArgs e)
        {

            outputVariantsChecked.Add(sender as CheckBox);
            CheckVariantManipulateButtons();
        }

        private void OnSelectInput(object sender, RoutedEventArgs e)
        {

            var s = sender as CheckBox;
            inputsChecked.Add(s);
            if (checkedOutput != null && s
                .Content.ToString().Equals(checkedOutput.Content.ToString()))
            {
                s.IsChecked = false;
            }
            creator.DataBase.TryAddInput(s.Content.ToString());
            Check();
        }
        private void OnDeselectInput(object sender, RoutedEventArgs e)
        {
            var s = sender as CheckBox;
            inputsChecked.Remove(s);
            creator.DataBase.RemoveInput(s.Content.ToString());
            Check();
        }
        private Button CreateButton(object content, RoutedEventHandler onClick)
        {
            var btn = new Button();
            btn.Content = content;
            btn.Click += onClick;
            return btn;
        }
        private void InitButtons()
        {
            deleteButton = CreateButton("Удалить", OnDeleteButtonClick);
            unionButton = CreateButton("Переименовать", OnUnionButtonClick);
            showButton = CreateButton("Показать предполагаемые значения классификатора",
                OnShowButtonClick);
            btnPanel = new Grid();
            btnPanel.ColumnDefinitions.Add(new ColumnDefinition());
            btnPanel.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(deleteButton, 0);
            Grid.SetColumn(unionButton, 1);
            btnPanel.Children.Add(deleteButton);
            btnPanel.Children.Add(unionButton);
        }
        private void InitLists()
        {
            inputsList = new ScrollViewer();
            outputsList = new ScrollViewer();
            outputVariantsList = new ScrollViewer();
        }
        private void OnSelectOutput(object sender, RoutedEventArgs e)
        {
            var s = sender as RadioButton;
            var oldName = creator.DataBase.OutputClassName;
            var newName = s.Content.ToString();
            checkedOutput = s;
            if (oldName != newName)
            {
                showButton.IsEnabled = true;
                creator.DataBase.Outputs.Clear();
                creator.DataBase.OutputClassName = checkedOutput.Content.ToString();
                UpdateOutputVariants();
                var inputWithSameName = (inputsList.Content as StackPanel)
                    .Children.Cast<CheckBox>().FirstOrDefault(x => x
                    .Content
                    .ToString().Equals(newName, StringComparison.OrdinalIgnoreCase));
                if (!(inputWithSameName is null)) 
                    inputWithSameName.IsChecked = false;
            }
            CheckVariantManipulateButtons();
            Check();

        }
        protected override Panel CreateContent()
        {
            creator = StepToken.GetContext<PerceptronCreator>();
            InitLabels();
            InitLists();
            InitButtons();
            InitContentContainer();
            SetRowsAndColumns();
            FillContentComtainer();
            return content;

        }

        private void FillContentComtainer()
        {
            content.Children.Add(label1);
            content.Children.Add(inputsList);
            content.Children.Add(label2);
            content.Children.Add(outputsList);
            content.Children.Add(label3);
            content.Children.Add(showButton);
            content.Children.Add(outputVariantsList);
            content.Children.Add(btnPanel);
        }

        private void SetRowsAndColumns()
        {
            Grid.SetRow(label1, 0);
            Grid.SetRow(inputsList, 1);
            Grid.SetRow(label2, 2);
            Grid.SetRow(outputsList, 3);
            Grid.SetRow(label3, 4);
            Grid.SetRow(showButton, 5);
            Grid.SetRow(outputVariantsList, 6);
            Grid.SetRow(btnPanel, 7);
        }

        private void InitContentContainer()
        {
            content = new Grid();
            GridLength pixelLength = new GridLength(20, GridUnitType.Pixel);
            content.RowDefinitions.Add(new RowDefinition() { Height = pixelLength });
            content.RowDefinitions.Add(new RowDefinition());
            content.RowDefinitions.Add(new RowDefinition() { Height = pixelLength });
            content.RowDefinitions.Add(new RowDefinition());
            content.RowDefinitions.Add(new RowDefinition() { Height = pixelLength });
            content.RowDefinitions.Add(new RowDefinition() { Height = pixelLength });
            content.RowDefinitions.Add(new RowDefinition());
            content.RowDefinitions.Add(new RowDefinition() { Height = pixelLength });
        }

        private void InitLabels()
        {
            label1 = new TextBlock();
            label1.Text = "Выберите входы нейросети";
            label2 = new TextBlock();
            label2.Text = "Выберите выход нейросети";
            label3 = new TextBlock();
            label3.Text = "Выберите имена классифицируемых параметров";
        }

        private void OnShowButtonClick(object sender, RoutedEventArgs e)
        {
            showButton.IsEnabled = false;
            creator.DataBase.Outputs.Clear();
            int limitCounter = 0;
            foreach (var variantName in creator.DataBase.GetOutputVariants())
            {
                if (++limitCounter > 101)
                {
                    break;
                }
                creator.DataBase.Outputs.Add(new OutputParameters(variantName));
            }
            UpdateOutputVariants();
        }

        private void OnUnionButtonClick(object sender, RoutedEventArgs e)
        {
            var name = outputVariantsChecked.Last().Content.ToString();
            UnionWindowViewModel union = new UnionWindowViewModel(name);
            if (union.ShowAsDialog().Value)
            {
                name = union.Name;
                foreach (var item in outputVariantsChecked)
                {
                    creator.DataBase.CollapseOutputs(name, item.Content.ToString());
                }
                UpdateOutputVariants();
            }
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in outputVariantsChecked)
            {
                creator.DataBase.RemoveOutput(item.Content.ToString());
            }
            UpdateOutputVariants();
        }
    }
}
