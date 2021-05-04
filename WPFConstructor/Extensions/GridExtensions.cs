using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFConstructor
{
    public static class GridExtensions
    {
        public static void AddColumns(this Grid grid, GridUnitType type, params double[] lengths)
        {
            for (int i = 0; i < lengths.Length; i++)
            {
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() 
                    { 
                        Width = new GridLength(lengths[i], type) 
                    });
            }
        }
        public static void AddRows(this Grid grid, GridUnitType type, params double[] lengths)
        {
            for (int i = 0; i < lengths.Length; i++)
            {
                grid.RowDefinitions.Add(
                    new RowDefinition()
                    {
                        Height = new GridLength(lengths[i], type)
                    });
            }
        }
    }
}
