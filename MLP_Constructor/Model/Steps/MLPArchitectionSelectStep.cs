using MLP_Constructor.Model.MLPParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFConstructor;

namespace MLP_Constructor.Model.Steps
{
    public class MLPArchitectionSelectStep : CustomStep
    {
        public override string Name => "Выбор конфигурации нейросети";
        private Grid content;

        private TextBox inputLayer;
        private TextBox outputLayer;

        private Button addButton;
        private Button confirmButton;
        private ScrollViewer hiddenContainer;
        private PerceptronCreator creator;
        protected override IEnumerable<CustomStep> CreateDependencies()
        {
            yield return GetInstance<DataPreprocessingStep>(StepToken);
        }

        protected override bool CheckComplete()
        {
            return !(creator.Perceptron is null);
        }
        protected override void UpdateContent()
        {
            confirmButton.IsEnabled = creator.Perceptron is null;
            inputLayer.Text = $"Входной слой: [{creator.DataBase.Inputs.Count}]";
            outputLayer.Text = $"Выходной слой: [{creator.DataBase.Outputs.Count}]";
            hiddenContainer.Content = CreateHiddenBlock();
        }
        private void UpdateHiddenInConstructor()
        {
            creator.HiddenLayers.Clear();
            var hiddenStack = hiddenContainer.Content as StackPanel;
            foreach (var item in hiddenStack.Children)
            {
                if (item is StackPanel miniStack)
                {
                    foreach (var miniItem in miniStack.Children)
                    {
                        if (miniItem is TextBox tb)
                        {
                            var size = int.Parse(tb.Text);
                            creator.HiddenLayers.Add(new HiddenLayerParameters(size));
                        }
                    }
                }
            }

        }
        private StackPanel CreateHiddenBlock()
        {
            StackPanel block = new StackPanel();
            int id = 0;
            foreach (var item in creator.HiddenLayers)
            {

                var hidden = new TextBlock();
                var size = new TextBox();
                var removeBtn = new Button();
                removeBtn.Content = "-";
                removeBtn.Width = 20;
                removeBtn.DataContext = item;
                removeBtn.Click += OnRemoveClick;
                var miniStack = new StackPanel();
                miniStack.Orientation = Orientation.Horizontal;
                hidden.Text = $"Скрытый слой: ";
                size.Text = item.Size.ToString();
                size.TextChanged += OnSizeInput;
                size.MinWidth = 30;
                miniStack.Children.Add(hidden);
                miniStack.Children.Add(size);
                miniStack.Children.Add(removeBtn);
                block.Children.Add(miniStack);
            }
            return block;
        }

        

        private void OnSizeInput(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (!int.TryParse(tb.Text, out var plug))
            {
                tb.Text = "1";
                return;
            }
            creator.ResetPerceptron();
            confirmButton.IsEnabled = true;
            Check();
        }

        protected override Panel CreateContent()
        {
            creator = StepToken.GetContext<PerceptronCreator>();
            hiddenContainer = new ScrollViewer();
            inputLayer = new TextBox();
            inputLayer.IsEnabled = false;
            outputLayer = new TextBox();
            outputLayer.IsEnabled = false;
            content = new Grid();
            addButton = new Button();
            addButton.Click += OnAddButtonClick;
            addButton.Content = "Добавить скрытый слой";
            confirmButton = new Button();
            confirmButton.Content = "Создать перцептрон";
            confirmButton.Click += OnConfirmButtonClick;
            content.AddRows(GridUnitType.Pixel, 30, 30);
            content.AddRows(GridUnitType.Star, 1);
            content.AddRows(GridUnitType.Pixel, 30, 30, 30);
            TextBlock label = new TextBlock();
            label.Text = "Настройте скрытые слои: ";

            Grid.SetRow(inputLayer, 1);
            Grid.SetRow(hiddenContainer, 2);
            Grid.SetRow(addButton, 3);
            Grid.SetRow(outputLayer, 4);
            Grid.SetRow(confirmButton, 5);
            content.Children.Add(label);
            content.Children.Add(inputLayer);
            content.Children.Add(hiddenContainer);
            content.Children.Add(addButton);
            content.Children.Add(outputLayer);
            content.Children.Add(confirmButton);
            return content;
        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateHiddenInConstructor();
            creator.TryCreate();
            confirmButton.IsEnabled = false;
            Check();
        }
        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var hidden = btn.DataContext as HiddenLayerParameters;
            creator.HiddenLayers.Remove(hidden);
            creator.ResetPerceptron();
            confirmButton.IsEnabled = true;
            Update(true);
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateHiddenInConstructor();
            creator.HiddenLayers.Add(new HiddenLayerParameters(1));
            creator.ResetPerceptron();
            confirmButton.IsEnabled = true;
            Update(true);
        }
    }
}
