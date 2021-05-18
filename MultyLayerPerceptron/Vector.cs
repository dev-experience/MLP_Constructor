using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron
{
    public class Vector : Matrix
    {
        public Vector(double[] values) : base(values.Length, 1, values)
        {

        }
        public int Length => Rows;
        public double this[int index]
        {
            get { return base[index, 0]; }
            set { base[index, 0] = value; }
        }
        public int MaxIndex()
        {
            int index = 0;
            double max;
            if (Length == 1)
            {
                return 0;
            }
            for (int i = 1; i < Length; i++)
            {
                max = this[index];
                if (this[i] >= max)
                {
                    index = i;
                }
            }
            return index;
        }
    }
}
