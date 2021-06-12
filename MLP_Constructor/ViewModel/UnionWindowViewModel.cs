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
    public class UnionWindowViewModel
    {
        private readonly UnionWindow window;
        public string Name { get; set; }
        public UnionWindowViewModel(string predictedName)
        {
            window = new UnionWindow();
            window.name.TextChanged += OnNameChanged;
            window.name.Text = predictedName;
            window.btn.Click += OnConfirm;
        }
        public bool? ShowAsDialog()
        {
            return window.ShowDialog();
        }
        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            Name = window.name.Text;
            window.DialogResult = true;
            window.Close();
        }

        private void OnNameChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if(tb.Text is null)
            {
                window.btn.IsEnabled = false;
            }
            else
            {
                window.btn.IsEnabled = true;
            }
        }
    }
}
