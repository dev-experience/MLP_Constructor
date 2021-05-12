using MultyLayerPerceptron.CalculatingGraph.GraphParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyLayerPerceptron.CalculatingGraph.Nodes
{
    public abstract class Node : IChildNode
    {
        public int BatchSize { get; set; }
        public void OnBatchSizeChanged(object sender, int newSize)
        {
            BatchSize = newSize;
            if (result is FakeBatch scaled)
            {
                scaled.SetNewSize(newSize);
            }
        }
        public Node(int batchSize = 1)
        {
            childs = new List<IChildNode>();
            BatchSize = batchSize;
        }
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }
        /// <summary>
        /// Дочерние узлы
        /// </summary>
        protected List<IChildNode> childs;
        protected Batch result;
        public virtual Batch Compute()
        {
            if (result == null)
            {

                result = ComputeForwardResult(Left?.Compute(), Right?.Compute());
            }
            return result;
        }
        public Batch Gradient { get; protected set; }

        public virtual bool IsCanResetResult => true;

        protected bool IsLast => childs.Count == 0;

        public Batch ComputeGradientByLeft()
        {
            return ComputeGradientByLeft(Gradient, Left?.Compute(), Right?.Compute());
        }
        public Batch ComputeGradientByRight()
        {
            return ComputeGradientByRight(Gradient, Left?.Compute(), Right?.Compute());
        }
        public Batch GetGradientByParent(INode parent)
        {
            GetOwnGradient();
            var parentSide = ParentSide(parent);
            switch (parentSide)
            {
                case ParentNodeSide.Left:
                    return ComputeGradientByLeft();
                case ParentNodeSide.Right:
                    return ComputeGradientByRight();
                case ParentNodeSide.None:
                    throw new InvalidOperationException(
                        $"Узел \"{this}\" не содержит родителя \"{parent}\"");
                default:
                    throw new InvalidOperationException(
                        $"Не удалось определить тип подключения " +
                        $"родительского узла \"{parent}\" к \"{this}\"");
            }
        }
        protected Batch GetOwnGradient()
        {

            if (Gradient is null)
            {
                if (IsLast)
                {
                    Gradient = Compute().ForEachMatrix(x => x.Fill(1));
                }
                else
                {
                    var grads = childs.Select(x => x.GetGradientByParent(this)).ToList();
                    Gradient = grads.Aggregate((x, y) => x.Sum(y));
                }
            }

            return Gradient;
        }
        public ParentNodeSide ParentSide(INode parentNode)
        {
            if (parentNode == Left)
            {
                return ParentNodeSide.Left;
            }
            else if (parentNode == Right)
            {
                return ParentNodeSide.Right;
            }
            else
            {
                return ParentNodeSide.None;
            }
        }
        protected abstract Batch ComputeGradientByLeft(Batch inputGradientResult, Batch leftResult, Batch rightResult);
        protected abstract Batch ComputeGradientByRight(Batch inputGradientResult, Batch leftResult, Batch rightResult);
        protected abstract Batch ComputeForwardResult(Batch leftResult, Batch rightResult);

        public void AddAsLeft(INode parent)
        {
            if (Left != null)
            {
                throw new InvalidOperationException();
            }
            Left = parent;
            parent.AddChild(this);
        }

        public void AddAsRight(INode parent)
        {
            if (Right != null)
            {
                throw new InvalidOperationException();
            }
            Right = parent;
            parent.AddChild(this);
        }

        public void AddChild(IChildNode node)
        {
            childs.Add(node);
        }

        public void ResetResult(bool force)
        {
            Gradient = null;
            if (IsCanResetResult || force)
            {
                result = null;
            }
        }

    }
}
