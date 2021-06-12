using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes
{
    public interface INode : IResultResetable
    {

        /// <summary>
        /// Результат прямой операции
        /// </summary>
        Batch Compute();
        /// <summary>
        /// Вычисленный градиент для текущего узла
        /// </summary>
        Batch Gradient { get; }
        Batch GetGradientByParent(INode parent);

        void AddChild(IChildNode node);
    }
}
