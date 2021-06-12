using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes
{
    public interface IChildNode : INode
    {
      
        
        /// <summary>
        /// Вычисляет градиент по левому родителю
        /// </summary>
        /// <returns>Результат вычисления градиента по левому родительскому узлу</returns>
        Batch ComputeGradientByLeft();
        /// <summary>
        /// Вычисляет градиент по правому родителю
        /// </summary>
        /// <returns>Результат вычисления градиента по правому родительскому узлу</returns>
        Batch ComputeGradientByRight();
        /// <summary>
        /// Проверяет тип подключения родительского узла
        /// </summary>
        /// <param name="parent">Предполагаемый родительский узел</param>
        /// <returns>Тип подключения родительского узла (слева/справа/не подключен)</returns>
        ParentNodeSide ParentSide(INode parent);

        void AddAsLeft(INode parent);
        void AddAsRight(INode parent);
    }
}
