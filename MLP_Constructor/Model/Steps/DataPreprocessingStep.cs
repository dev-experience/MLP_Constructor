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
    public class DataPreprocessingStep : CustomStep
    {
        public override string Name => "Создание контейнера для данных";
        private PerceptronCreator creator;
        private Button relocateButton;
        private ScrollViewer inputs;
        private TextBlock label1;
        private bool isProcessing;

        protected override IEnumerable<CustomStep> CreateDependencies()
        {
            yield return GetInstance<InputOutputSelectionStep>(StepToken);
        }
        protected override bool CheckComplete()
        {

            var cond = creator.DataBase.IsTableExist(true);
            cond = cond && creator.DataBase.IsCorrect;
            return cond && !isProcessing;
        }
        private StackPanel CreateInputs()
        {
            StackPanel container = new StackPanel();
            foreach (var item in creator.DataBase.Inputs)
            {
                TextBlock inp = new TextBlock();
                inp.Text = $"{item.Name}: [ {item.MinValue} ; {item.MaxValue} ]";
                container.Children.Add(inp);
            }
            return container;
        }
        protected override void UpdateContent()
        {
            ConstructButton();
            ConstructLabel();
            inputs.Content = CreateInputs();
        }
        private void ConstructLabel()
        {

            var cond = creator.DataBase.IsTableExist(true);
            if (cond)
            {
                label1.Text = "Выходная таблица: " + creator.DataBase.FormattedTableName();
            }
            else
            {
                label1.Text = "Выходная таблица отсутствует";
            }

        }
        private void ConstructButton()
        {
            if (creator.DataBase.IsTableExist(true))
            {
                relocateButton.Content = "Определить границы нормализации";
            }
            else
            {
                relocateButton.Content = "Создать новую таблицу";

            }
        }
        protected override Panel CreateContent()
        {
            inputs = new ScrollViewer();
            label1 = new TextBlock();

            creator = StepToken.GetContext<PerceptronCreator>();
            var content = new Grid();
            content.AddRows(GridUnitType.Pixel, 30, 30);
            content.AddRows(GridUnitType.Star, 1);

            relocateButton = new Button();

            relocateButton.Click += OnCreateTableButtonClick;
            Grid.SetRow(relocateButton, 0);
            Grid.SetRow(label1, 1);
            Grid.SetRow(inputs, 2);
            content.Children.Add(relocateButton);
            content.Children.Add(label1);
            content.Children.Add(inputs);
            return content;
        }

        private async void OnCreateTableButtonClick(object sender, RoutedEventArgs e)
        {
            isProcessing = true;
            relocateButton.IsEnabled = false;
            if (!creator.DataBase.IsTableExist(true))
            {
                creator.DataBase.CreateTable();
                relocateButton.Content = "Идет перенос данных в новую таблицу";
                await creator.DataBase.RelocateAsync();
            }
            relocateButton.Content = "Идет определение диапазонов нормализации";
            await creator.DataBase.NormalizeAsync();

            relocateButton.IsEnabled = true;
            isProcessing = false;
            Update(true);

        }
    }
}
