using MLP_Constructor.Model;
using MLP_Constructor.Model.MLPParameters;
using MLP_Constructor.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MLP_Constructor.ViewModel
{
    public class PerceptronNameWindowViewModel
    {
        private readonly IEnumerable<string> existedNames;

        private readonly PerceptronNameWindow window;
        public string Name => window.nameBox.Text;
        public PerceptronNameWindowViewModel(IEnumerable<string> existedNames)
        {
            window = new PerceptronNameWindow();
            window.HorizontalAlignment = HorizontalAlignment.Center;
            window.nameBox.TextChanged += OnTextChanged;
            window.nameButton.Click += OnConfirm;
            this.existedNames = existedNames;
        }

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            window.DialogResult = true;
            window.Close();
        }

        public bool? ShowAsDialog()
        {
            return window.ShowDialog();
        }
        private bool IsValid(string name, out string comment)
        {
            if (name.Equals(""))
            {
                comment = "Имя не может быть пустым";
                return false;
            }
            if (existedNames == null || existedNames.Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                comment = "Персептрон с таким именем уже существует";
                return false;
            }
            
            if (!Regex.IsMatch(name, "^[ a-zа-яА-ЯA-Z0-9]+$"))
            {
                comment = "Некорректное имя";
                return false;
            }

            comment = "Приемлемое имя";
            return true;
        }
        private bool Check()
        {
            var isValid = IsValid(window.nameBox.Text, out var comment);
            window.error.Foreground = isValid ? Brushes.Green : Brushes.Red;
            window.error.Text = comment;
            return isValid;
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            window.nameButton.IsEnabled = Check();
        }

    }
}
