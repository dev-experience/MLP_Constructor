using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.GraphParameters
{
    class FakeBatch : Batch
    {
        private Matrix value;
        public override Matrix this[int index]
        {
            get => value;
            set => this.value = value;
        }
        public FakeBatch(int size, Matrix value)
        {
            SetNewSize(size);
            this.value = value;
        }
        public override Batch Sum(Batch second)
        {
            if (!IsSameDimension(second))
            {
                throw new InvalidOperationException();
            }
            if(second is FakeBatch secondFake)
            {
                return new FakeBatch(second.Size, value.Sum(secondFake[0]));
            }
            else
            {
                return second.Sum(this);
            }

        }
        public override Batch ForEachMatrix(Func<Matrix, Matrix> operation)
        {
            var buf = value.Clone() as Matrix;
            return new FakeBatch(Size, operation(buf));
        }
        public override Batch ForEachMatrix(Batch second, Func<Matrix, Matrix, Matrix> operation)
        {
            if(second is FakeBatch secondFake)
            {
                return new FakeBatch(secondFake.Size, operation(value, secondFake.value));
            }
            return base.ForEachMatrix(second, operation);
        }
        public static FakeBatch operator*(FakeBatch b,double v)
        {
            return new FakeBatch(b.Size, b.value * v);
        }
    }
}
