using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron
{
    public abstract class Operation<TIn,TOut>
    {
        public Func<TIn, TOut> Forward { get; }
        public Func<TOut, TIn> Back { get; }
        protected abstract Func<TIn, TOut> ConstructForward();
        protected abstract Func<TOut, TIn> ConstructBack();

        public Operation()
        {
            Forward = ConstructForward();
            Back = ConstructBack();
        }
    }
}
