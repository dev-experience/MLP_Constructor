using MLP_Constructor.Model;
using MLP_Constructor.Model.EntityDataModel;
using MLP_Constructor.Model.MLPParameters;
using MLP_Constructor.Model.Stages;
using MLP_Constructor.Model.Supported;
using MLP_Constructor.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFConstructor;

namespace MLP_Constructor.ViewModel
{

    public class MainWindowViewModel
    {
        public StepByStepToken Token { get; set; }
        public MainWindow MainWindow { get; set; }
        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new SelectPerceptronWindowViewModel();
            if (openFileDialog.ShowAsDialog() == true)
            {

                Token.DataContext = openFileDialog.Selected;
                MainWindow.Title = $"Конструктор нейросетей - {openFileDialog.Selected.Name}";
                Token.InteractWithConstructor(x => x.SetShowContent(true));
            }
        }


        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (!(Token.DataContext is null))
            {
                DataBase.UploadCreator(Token.GetContext<PerceptronCreator>());
                Token.DataContext = null;
            }
            Token.InteractWithConstructor(x => x.SetShowContent(false));
        }
        public MainWindowViewModel(MainWindow mainWindow)
        {
            mainWindow.Title = "Конструктор нейросетей";
            MainWindow = mainWindow;
            Token = StepByStepWPFConstructor.New(MainWindow.mainGrid,
                new MLPStagesFactory(), null);
            Token.InteractWithConstructor(x => x.SetOnOpen(OnOpenClick));
            Token.InteractWithConstructor(x => x.SetOnSave(OnSaveClick));

        }
        public void Close()
        {
            OnSaveClick(null, null);
        }
    }
}
