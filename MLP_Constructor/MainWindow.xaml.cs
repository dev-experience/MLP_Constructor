using MLP_Constructor.ViewModel;
using System.ComponentModel;
using System.Windows;
using WPFConstructor;
using WPFConstructor.Steps;

namespace MLP_Constructor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            (DataContext as MainWindowViewModel).Close();
            base.OnClosing(e);
        }
    }

}
