using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.GraphParameters
{
    public class Input<T> : Input
    {
        public string Name { get; set; }
        private readonly T minValue;
        private readonly T maxValue;
        private readonly INormalizeProvider<T> normalizeProvider;
        public double NormalizeValue { get; private set; }
        public T Value { get; private set; }

        public Input(T min, T max, INormalizeProvider<T> normalizeProvider)
        {
            this.normalizeProvider = normalizeProvider;
            minValue = min;
            maxValue = max;
        }
        public override double Init(object value) 
        {
            Value = (T)value;
            NormalizeValue = normalizeProvider.Normalize(Value, minValue, maxValue);
            return NormalizeValue;
        }
    }
    public abstract class Input
    {
        public abstract double Init(object value);

    }

}
