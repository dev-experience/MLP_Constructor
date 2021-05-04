using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFConstructor.Steps
{
    public class ReadmeStep1 : Step
    {
        public override string Name => "Тест";

        protected override Panel CreateContent()
        {
            TextBlock text = new TextBlock();
            text.Text = StepToken.GetContext<string>();
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.Children.Add(text);
            return grid;
        }
    }
}
