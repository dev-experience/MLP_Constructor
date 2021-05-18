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

    public class TrainPerceptronStep : CustomStep
    {
        public override string Name => "Тренеровка нейросети";
        private Button trainButton;
        private Button checkButton;
        private ScrollViewer trainList;
        private ScrollViewer checkList;
        private StackPanel checkContainer;
        private StackPanel trainContainer;
        private PerceptronCreator creator;
        private Trainer trainer;
        protected override IEnumerable<CustomStep> CreateDependencies()
        {
            yield return GetInstance<MLPArchitectionSelectStep>(StepToken);
        }
        protected override void UpdateContent()
        {
            trainButton.Content = creator.IsTrained ? "Сеть обучена." : "Обучить сеть.";
            trainButton.IsEnabled = !creator.IsTrained;
            if (creator.DataBase.IsTableExist(true))
            {
                trainer = new Trainer(creator);
            }
        }
        protected override Panel CreateContent()
        {
            creator = StepToken.GetContext<PerceptronCreator>();
            checkContainer = new StackPanel();
            trainContainer = new StackPanel();
            trainList = new ScrollViewer();
            trainList.Content = trainContainer;
            checkList = new ScrollViewer();
            checkList.Content = checkContainer;
            trainButton = new Button();
            trainButton.Click += OnTrainButtonClick;
            checkButton = new Button();
            checkButton.Click += OnCheckButtonClick;
            checkButton.Content = "Проверить обобщаемость сети";
            var content = new Grid();
            content.AddRows(GridUnitType.Pixel, 30);
            content.AddRows(GridUnitType.Star, 1);
            content.AddRows(GridUnitType.Pixel, 30);
            content.AddRows(GridUnitType.Star, 1);
            Grid.SetRow(trainButton, 0);
            Grid.SetRow(trainList, 1);
            Grid.SetRow(checkButton, 2);
            Grid.SetRow(checkList, 3);
            content.Children.Add(trainButton);
            content.Children.Add(trainList);
            content.Children.Add(checkButton);
            content.Children.Add(checkList);
            return content;

        }

        private void OnCheckButtonClick(object sender, RoutedEventArgs e)
        {
            if (!creator.IsTrained)
            {
                var tb = new TextBlock();
                tb.Text = $"Сеть не обучена";

            }
            checkButton.IsEnabled = false;
            while (trainer.Train())
            {
                var tb = new TextBlock();
                tb.Text = $"Текущая ошибка: {trainer.Error * 100}%";
                checkContainer.Children.Add(tb);
                checkList.ScrollToBottom();
            }
        }
        private async void OnTrainButtonClick(object sender, RoutedEventArgs e)
        {
            trainButton.IsEnabled = false;
            var tb = new TextBlock();
            trainContainer.Children.Add(tb);
            while (await trainer.TrainAsync())
            {
                tb.Text = $"Текущая ошибка: {trainer.Error * 100}%";
                //trainContainer.Children.Clear();
               // trainList.ScrollToBottom();
            }
            creator.IsTrained = true;
        }
    }
}
