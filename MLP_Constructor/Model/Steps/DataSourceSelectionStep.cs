using Microsoft.Win32;
using MLP_Constructor.Model.MLPParameters;
using MLP_Constructor.Model.Supported;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFConstructor;

namespace MLP_Constructor.Model.Steps
{
    public class DataSourceSelectionStep : CustomStep
    {
        public override string Name => "Выбор источника данных";
        private PerceptronCreator creator;
        private ScrollViewer tableContainer;
        private TextBox fileName;
        protected override bool CheckComplete()
        {
            var db = creator.DataBase;
            bool res = db.GetTableNames().Contains(db.TableName);
            return res;
        }
        private StackPanel CreateTablesList()
        {
            StackPanel tables = new StackPanel();
            tables.CanHorizontallyScroll = true;
            foreach (var tableName in creator.DataBase.GetTableNames())
            {
                RadioButton table = new RadioButton();
                table.GroupName = "tables";
                table.Content = tableName;
                table.IsChecked = tableName.Equals(creator.DataBase.TableName,
                    StringComparison.OrdinalIgnoreCase);
                table.Checked += OnSelectTable;
                tables.Children.Add(table);
            }
            return tables;
        }
        private void UpdateTables()
        {
            tableContainer.Content = CreateTablesList();
        }
        protected override void UpdateContent()
        {
            UpdateTables();
            UpdateFileName();
        }
        protected override Panel CreateContent()
        {
            creator = StepToken.GetContext<PerceptronCreator>();
            Grid content = new Grid();
            content.AddColumns(GridUnitType.Star, 1, 3);
            content.AddRows(GridUnitType.Pixel, 20, 20, 20);
            content.AddRows(GridUnitType.Star, 1);
            fileName = new TextBox();
            fileName.IsEnabled = false;
            Button selectFileButton = new Button();
            selectFileButton.Content = "Выбрать файл";


            selectFileButton.Click += OnSelectFileButtonClick;
            tableContainer = new ScrollViewer();


            TextBlock label1 = new TextBlock();
            label1.Text = "Выберите файл базы данных:";
            TextBlock label2 = new TextBlock();
            label2.Text = "Выберите таблицу с данными:";

            Grid.SetColumn(fileName, 1);
            Grid.SetRow(fileName, 1);
            Grid.SetRow(selectFileButton, 1);
            Grid.SetRow(label2, 2);
            Grid.SetRow(tableContainer, 3);
            Grid.SetColumnSpan(label1, 2);
            Grid.SetColumnSpan(label2, 2);
            Grid.SetColumnSpan(tableContainer, 2);

            content.Children.Add(label1);
            content.Children.Add(selectFileButton);
            content.Children.Add(fileName);
            content.Children.Add(label2);
            content.Children.Add(tableContainer);
            
            return content;
        }

        private void UpdateFileName()
        {
            var fileSource = creator.DataBase.Source;
            if (creator.DataBase.IsCorrectSource())
            {
                fileName.Text = fileSource;
            }
            else
            {
                fileName.Text = "Не выбрано";
            }
        }

        private void OnSelectTable(object sender, RoutedEventArgs e)
        {
            StepToken.InteractWithDataContext<PerceptronCreator>(x => x
            .DataBase.TableName = (sender as RadioButton).Content.ToString());
            Check();
        }

        private void OnSelectFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = ProviderSelector.GetAvaliableFilter();
            if (dlg.ShowDialog() == true)
            {
                StepToken.GetContext<PerceptronCreator>().
                    DataBase.Source = dlg.FileName;
                Reload();

            }

        }
    }
}
