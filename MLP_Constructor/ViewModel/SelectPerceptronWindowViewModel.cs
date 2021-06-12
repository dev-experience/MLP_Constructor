using MLP_Constructor.Model;
using MLP_Constructor.Model.EntityDataModel;
using MLP_Constructor.Model.MLPParameters;
using MLP_Constructor.Model.Supported;
using MLP_Constructor.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MLP_Constructor.ViewModel
{
    public class SelectPerceptronWindowViewModel
    {
        public PerceptronCreator Selected { get; private set; }
        private readonly SelectPerceptronWindow window;
        public SelectPerceptronWindowViewModel()
        {
            window = new SelectPerceptronWindow();
            window.DataContext = this;
            window.Title = "Открыть файл";
            ConstructButtons();
        }
        public bool? ShowAsDialog()
        {
           // window.Show();
            return window.ShowDialog();
        }

        private Button CreateButton(PerceptronCreator perceptron)
        {
            var button = new Button();
            button.DataContext = perceptron;
            button.Content = perceptron.ToString();
            button.Click += OnSelected;
            button.Height = 50;
            //   button.VerticalAlignment = VerticalAlignment.Center;
            //   button.HorizontalAlignment = HorizontalAlignment.Center;
            return button;
        }
        private void ConstructButtons()
        {

            var btn = CreateButton(new PerceptronCreator());
            btn.Content = "Создать";
            btn.Click -= OnSelected;
            btn.Click += OnCreateSelected;
            window.perceptronsContainer.Children.Add(btn);
            foreach (var item in DataBase.LoadSortedPerceptronCreators())
            {
                window.perceptronsContainer.Children.Add(CreateButton(item));
            }
        }

        private void OnCreateSelected(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var creator = btn.DataContext as PerceptronCreator;
            var nameDialog = new PerceptronNameWindowViewModel(
                DataBase.LoadPerceptronCreators().Select(x => x.Name));
            if (nameDialog.ShowAsDialog() == true)
            {
                creator.Name = nameDialog.Name;
                OnSelected(sender, e);
            }
        }

        private void OnSelected(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            Selected = btn.DataContext as PerceptronCreator;
            window.DialogResult = true;
            window.Close();
        }
    }
}
