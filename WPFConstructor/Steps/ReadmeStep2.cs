using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFConstructor.Steps
{
    public class ReadmeStep2 : Step
    {
        public override string Name => "Продолжение";

        protected override Panel CreateContent()
        {
            var grid = new Grid();
            grid.Children.Add(new Button());
            return grid;
        }
    }
}
