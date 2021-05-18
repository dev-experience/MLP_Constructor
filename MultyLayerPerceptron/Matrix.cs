using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron
{
    public class Matrix : ISummable<Matrix>, ICloneable
    {
        public double this[int row, int col]
        {
            get { return Values[row * Columns + col]; }
            set { Values[row * Columns + col] = value; }
        }
        public override string ToString()
        {
            return $"[{Rows}x{Columns}] " + base.ToString();
        }
        public void InsertRow(int row, params double[] values)
        {
            Parallel.For(0, Columns, (i) =>
            {
                Values[row * Columns + i] = values[i];
            });

        }
        public void InsertColumn(int column, params double[] values)
        {
            Parallel.For(0, Rows, (i) =>
            {
                Values[i * Columns + column] = values[i];
            });
        }
        public Vector AsVector => TryTransformToVector();

        private Vector TryTransformToVector()
        {
            if (Columns != 1)
            {
                throw new InvalidCastException("Не вектор");
            }
            return new Vector(Values.Clone() as double[]);
        }
        public Matrix Fill(double value)
        {
            return OperationForEachElement((plug) => value);
        }
        private bool IsScalar()
        {
            return Rows == 1 && Columns == 1;
        }
        public Matrix ElementByElement(Matrix second, Func<double, double, double> operation)
        {
            if (second.IsScalar())
            {
                second = new Matrix(Rows, Columns, null).Fill(second[0, 0]);
            }
            if (Rows != second.Rows || Columns != second.Columns)
            {
                throw new InvalidOperationException();
            }
            var first = new Matrix(Rows, Columns, Values);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int row = i;
                    int col = j;
                    first[row, col] = operation(first[row, col], second[row, col]);
                }
            }
            return first;
        }

        public void Check()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (double.IsNaN(Values[i]))
                {
                    throw new Exception("NaN");
                }
                if (double.IsInfinity(Values[i]))
                {
                    throw new Exception("infinity");
                }
            }
        }

        public Matrix ElementByElementMultiplication(Matrix second)
        {

            return ElementByElement(second, (x, y) => x * y);
        }
        public Matrix Transposed()
        {

            var matrix = new Matrix(Columns, Rows, new double[] { });
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    matrix[col, row] = this[row, col];
                }
            }
            return matrix;
        }
        public double PoweredNormF()
        {
            double sum = 0;
            foreach (var item in Values)
            {
                sum += item * item;
            }
            return sum;
        }
        public double NormF()
        {
            return Math.Sqrt(PoweredNormF());
        }
        public Matrix OperationForEachElement(Func<double, double> operation)
        {
            var res = this.Clone() as Matrix;
            Parallel.For(0, Values.Length, (i) =>
              {
                  res.Values[i] = operation(Values[i]);
              });
            return res;
        }
        public static Matrix Rand(int rows, int cols, double min = 0, double max = 1)
        {
            Random rnd = new Random();
            if (rows < 1 || cols < 1)
            {

                throw new ArgumentException($"{nameof(rows)} и {nameof(cols)} должны быть больше 0)");
            }
            if (min > max)
            {
                throw new ArgumentException($"{nameof(min)} должно быть меньше {nameof(max)}");
            }
            var values = new double[rows * cols];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = min + rnd.NextDouble() * (max - min);
            }
            return new Matrix(rows, cols, values);
        }

        public Matrix Sum(Matrix second)
        {
            return this + second;
        }


        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public double[] Values { get; private set; }
        public static Matrix Scalar(double value)
        {
            return new Matrix(1, 1, new double[] { value });
        }
        public static Matrix Vector(double[] values)
        {
            return new Matrix(values.Length, 1, values);
        }

        public object Clone()
        {
            return new Matrix(Rows, Columns, Values);
        }

        public Matrix(int rows, int columns, double[] values)
        {
            if (values is null)
            {
                values = new double[] { };
            }

            Values = new double[rows * columns];

            for (int i = 0; i < Values.Length && i < values.Length; i++)
            {
                Values[i] = values[i];
            }
            Rows = rows;
            Columns = columns;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.Rows != m2.Rows || m1.Columns != m2.Columns)
            {
                throw new InvalidOperationException(
                    $"Размеры матриц должны совпадать");

            }
            var resValues = m1.Values;
            for (int i = 0; i < resValues.Length; i++)
            {
                resValues[i] += m2.Values[i];
            }
            return new Matrix(m1.Rows, m1.Columns, resValues);
        }
        public static Matrix operator -(Matrix m)
        {
            var values = m.Values;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = -values[i];
            }
            return new Matrix(m.Rows, m.Columns, values);
        }
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            return m1 + (-m2);
        }
        public static Matrix operator *(Matrix m1, double n)
        {
            return m1.OperationForEachElement(x => n * x);
        }
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.Columns != m2.Rows)
            {
                throw new InvalidOperationException($"Количество столбцов первой матрицы и " +
                    $"количество строк второй матрицы должны совпадать");
            }
            double[] resultValues = new double[m1.Rows * m2.Columns];
            Matrix res = new Matrix(m1.Rows, m2.Columns, null);
            Parallel.For(0, m2.Columns, (col2) =>
              {
                  for (int row1 = 0; row1 < m1.Rows; row1++)
                  {
                      for (int col1 = 0; col1 < m1.Columns; col1++)
                      {
                          int row2 = col1;
                          res[row1, col2] += m1[row1, col1] * m2[row2, col2];
                      }
                  }
              });

            return res;
        }
        public static implicit operator Matrix(double value)
        {
            return new Scalar(value);
        }
        public static implicit operator Matrix(double[] value)
        {
            return new Vector(value);
        }

    }
}
