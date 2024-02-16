using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security;
using System.Xml.Linq;

namespace BinarySearchTree
{
    public enum Side
    {
        Left,
        Right
    }

    public class BinaryTreeNode<T>
        where T : IComparable, ICloneable
    {
        public BinaryTreeNode(T data)
        {
            Data = data;
        }

        public T Data { get; set; }

        public BinaryTreeNode<T>? LeftNode { get; set; }

        public BinaryTreeNode<T>? RightNode { get; set; }

        public override string? ToString() => Data.ToString();
    }

    public class BinaryTree<T> : ICollection<T>, IEnumerable<T>, ICloneable
        where T : IComparable, ICloneable
    {
        #region Local variables/constants


        #endregion

        #region Constructors
        public BinaryTree(IComparer<T>? comparer)
        {
            this.Comparer = comparer ?? Comparer<T>.Default;
        }

        public BinaryTree() : this(Comparer<T>.Default) { }


        public BinaryTree(BinaryTree<T> tree) : this(tree.Comparer)
        {
            if (tree.Count != 0)
            {
                this.RootNode = CloneNode(tree.RootNode);
                this.Count = tree.Count;
            }
        }

        public BinaryTree(IEnumerable<T> collection, IComparer<T>? comparer = null) : this(comparer)
        {
            AddRange(collection);
        }
        #endregion

        #region Properties

        public BinaryTreeNode<T>? RootNode { get; set; }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public IComparer<T> Comparer { get; private set; }

        #endregion

        #region ICollection<T> members

        public BinaryTreeNode<T> Add(BinaryTreeNode<T>? currentNode, T data)
        {
            if (currentNode == null)
                return new BinaryTreeNode<T>(data);

            int result = Comparer.Compare(currentNode.Data, data);

            if (result == 0)
                throw new InvalidOperationException("Бинарное дерево не содержит дубликатов данных.");
            else if (result > 0)
                currentNode.LeftNode = Add(currentNode.LeftNode, data);
            else if (result < 0)
                currentNode.RightNode = Add(currentNode.RightNode, data);
            return currentNode;
        }

        public void Add(T data)
        {
            RootNode = Add(RootNode, data);
            Count++;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var value in collection)
                Add(value);
        }

        public bool Remove(T data)
        {
            var initialCount = Count;
            RootNode = Remove(RootNode, data);
            return Count < initialCount;
        }

        private BinaryTreeNode<T>? Remove(BinaryTreeNode<T>? currentNode, T data)
        {
            if (currentNode == null)
                return null;

            int result = Comparer.Compare(currentNode.Data, data);

            if (result < 0)
                currentNode.LeftNode = Remove(currentNode.LeftNode, data);
            else if (result > 0)
                currentNode.RightNode = Remove(currentNode.RightNode, data);
            else
            {
                if (currentNode.LeftNode == null)
                {
                    --Count;
                    return currentNode.RightNode;
                }
                else if (currentNode.RightNode == null)
                {
                    --Count;
                    return currentNode.LeftNode;
                }

                currentNode.Data = GetMinValue(currentNode.RightNode);
                currentNode.RightNode = Remove(currentNode.RightNode, currentNode.Data);
            }
            return currentNode;
        }
        private static T GetMinValue(BinaryTreeNode<T> currentNode)
        {
            while (currentNode.LeftNode != null)
                currentNode = currentNode.LeftNode;

            return currentNode.Data;
        }

        public void Clear()
        {
            RootNode = null;
            Count = 0;
        }

        public bool Contains(T item)
        {
            return FindNode(item, RootNode) is not null;
        }

        public T? Find(T item)
        {
            var itemNode = FindNode(item, RootNode);
            if (itemNode != null)
                return itemNode.Data;
            return default;
        }

        /// <summary>
        /// Поиск узла по значению
        /// </summary>
        /// <param name="data">Искомое значение</param>
        /// <param name="startWithNode">Узел начала поиска</param>
        /// <returns>Найденный узел</returns>
        public BinaryTreeNode<T>? FindNode(T data, BinaryTreeNode<T>? startWithNode = null)
        {
            if (startWithNode == null)
                return null;

            int result = Comparer.Compare(data, startWithNode.Data);

            if (result == 0)
                return startWithNode;
            else if (result < 0)
                return FindNode(data, startWithNode.LeftNode);
            else
                return FindNode(data, startWithNode.RightNode);

        }

        public void CopyTo(T[]? array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Недостаточное пространство в целевом массиве.");
            }

            foreach (var value in this)
                array[arrayIndex++] = value;
        }

        #endregion

        #region IEnumerable<T> members
        public IEnumerator<T> GetEnumerator()
        {
            return InOrder(RootNode).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private IEnumerable<T> InOrder(BinaryTreeNode<T>? node)
        {
            if (node != null)
            {
                foreach (var item in InOrder(node.LeftNode))
                {
                    yield return item;
                }

                yield return node.Data;

                foreach (var item in InOrder(node.RightNode))
                {
                    yield return item;
                }
            }
        }

        #endregion

        #region ICloneable members
        public object ShallowCopy()
        {
            return (BinaryTree<T>)this.MemberwiseClone();
        }

        public object Clone()
        {
            return (object)new BinaryTree<T>(this);
        }

        private BinaryTreeNode<T>? CloneNode(BinaryTreeNode<T>? node)
        {
            if (node == null)
                return null;


            var clonedNode = new BinaryTreeNode<T>((T)node.Data.Clone())
            {
                LeftNode = CloneNode(node.LeftNode),
                RightNode = CloneNode(node.RightNode)
            };

            return clonedNode;
        }


        #endregion

        #region Other methods

        public void PrintTree()
        {
            if (Count == 0)
                Console.WriteLine("Дерево пустое");
            else
                PrintTree(RootNode);
        }


        private void PrintTree(BinaryTreeNode<T>? startNode, string indent = "", Side? side = null)
        {
            if (startNode != null)
            {
                var nodeSide = side == null ? "+" : side == Side.Left ? "L" : "R";
                Console.WriteLine($"{indent} [{nodeSide}]- {startNode.Data}");
                indent += new string(' ', 3);

                PrintTree(startNode.LeftNode, indent, Side.Left);
                PrintTree(startNode.RightNode, indent, Side.Right);
            }
        }
        #endregion


    }

}