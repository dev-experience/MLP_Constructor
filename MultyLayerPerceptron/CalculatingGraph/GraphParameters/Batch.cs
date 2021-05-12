using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.GraphParameters
{
    public class Batch
    {
        private Matrix[] values;
        public int Size { get; private set; }


        public Batch(params Matrix[] values)
        {
            this.values = values;
            SetNewSize(values.Length);
        }
        public Batch ElementByElement(Batch second, Func<double, double, double> operation)
        {
            if (Size == 0)
            {
                throw new Exception("Размер мини-пакета равен нулю");
            }
            var res = new Batch(new Matrix[Size]);
            for (int i = 0; i < Size; i++)
            {
                var a = this[i].ElementByElement(second[i], operation);
                res[i] = a;
            }
            return res;
        }
        public virtual Matrix this[int index]
        {
            get { return values[index]; }
            set
            {
                if (values is Vector[])
                {
                    values[index] = value.AsVector;
                    return;
                }
                values[index] = value;

            }
        }
        public void SetNewSize(int size)
        {
            Size = size;
        }
        protected bool IsSameDimension(Batch second)
        {
            return Size == second.Size;
        }
        public virtual Batch Sum(Batch second)
        {
            if (!IsSameDimension(second))
            {
                throw new InvalidOperationException();
            }
            var resBatch = new Batch(new Matrix[Size]);
            Parallel.For(0, Size, (i) =>
            {
                resBatch[i] = values[i].Sum(second[i]);
            });
            return resBatch;
        }
        public virtual Batch ForEachMatrix(Func<Matrix, Matrix> operation)
        {
            var res = new Batch(new Matrix[Size]);
            Parallel.For(0, Size, (i) =>
            {
                res[i] = operation(this[i]);
            });

            return res;
        } 
        public virtual Batch ForEachMatrix(Batch second, Func<Matrix, Matrix, Matrix> operation)
        {
            var res = new Batch(new Matrix[Size]);
            for (int i = 0; i < Size; i++)
            {
                res[i] = operation(this[i].Clone() as Matrix, second[i]);
            }
            return res;
        }
        public static Batch operator *(Batch b1, Batch b2)
        {
            return b1.ForEachMatrix(b2, (x, y) => x * y);
        }
        public static Batch operator *(Batch b, double value)
        {
            return b.ForEachMatrix((x) => x * value);
        }
        public static Batch operator +(Batch b1, Batch b2)
        {
            return b1.ForEachMatrix(b2, (x, y) => x + y);
        }
        public override string ToString()
        {
            return $"{Size} " + base.ToString();
        }

    
    }
}
