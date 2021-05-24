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
        private ScrollViewer trainList;
        private StackPanel checkContainer;
        private StackPanel trainContainer;
        private PerceptronCreator creator;
        private Trainer trainer;
        private double sum = 0;
        private double experimentCount = 0;
        private List<double> last = new List<double>();

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
            trainButton = new Button();
            trainButton.Click += OnTrainButtonClick;
            var content = new Grid();
            content.AddRows(GridUnitType.Pixel, 30);
            content.AddRows(GridUnitType.Star, 1);
            Grid.SetRow(trainButton, 0);
            Grid.SetRow(trainList, 1);
            content.Children.Add(trainButton);
            content.Children.Add(trainList);
            return content;

        }


        private async void OnTrainButtonClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                last.Add(100);
            }
            trainButton.IsEnabled = false;
            while (await trainer.TrainAsync())
            {
                var tb = new TextBlock();
                var error = trainer.Error * 100;
                sum += error;
                experimentCount++;
                last.Add(error);
                last.Remove(last.First());
                tb.Text = $"[{experimentCount}] Успешное обучение мини-пакета.";

                //$"Текущая ошибка: {error}%. " +
                //$"Avg: {sum / experimentCount}%. " +
                //$"Last100 avg: {last.Aggregate((x,y)=>x+y)/100}";
                trainContainer.Children.Add(tb);
                if (experimentCount % 100 == 0)
                {
                    trainContainer.Children.RemoveRange(0, 90);
                }
                trainList.ScrollToBottom();
            }
            creator.IsTrained = true;
            DataBase.UpdatePerceptron(creator);
        }

    }
}
