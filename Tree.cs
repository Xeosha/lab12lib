using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Trees
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

        public BinaryTreeNode<T>? ParentNode { get; set; }

        /// <summary>
        /// Расположение узла относительно его родителя
        /// </summary>
        public Side? NodeSide =>
           ParentNode == null
           ? (Side?)null
           : ParentNode.LeftNode == this
               ? Side.Left
               : Side.Right;

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

        /// <summary>
        /// Добавление нового узла в бинарное дерево
        /// </summary>
        /// <param name="node">Новый узел</param>
        /// <param name="currentNode">Текущий узел</param>
        /// <returns>Узел</returns>
        public BinaryTreeNode<T> Add(BinaryTreeNode<T> node, BinaryTreeNode<T>? currentNode = null)
        {
            if (RootNode == null)
            {
                node.ParentNode = null;
                return RootNode = node;
            }

            currentNode ??= RootNode;
            node.ParentNode = currentNode;

            int result = Comparer.Compare(node.Data, currentNode.Data);

            if (result < 0)
            {
                if (currentNode.LeftNode == null)
                {
                    currentNode.LeftNode = node;
                }
                else
                {
                    Add(node, currentNode.LeftNode);
                }
            }
            else if (result > 0)
            {
                if (currentNode.RightNode == null)
                {
                    currentNode.RightNode = node;
                }
                else
                {
                    Add(node, currentNode.RightNode);
                }
            }
            return currentNode;
        }

        public void Add(T data)
        {
            Add(new BinaryTreeNode<T>(data));
            Count++;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var value in collection)
                Add(value);
        }

        public bool Remove(T item)
        {
            if (RootNode == null)
                return false;

            BinaryTreeNode<T>? current = RootNode, parent = null;

            int result;
            do
            {
                result = Comparer.Compare(item, current.Data);
                if (result < 0)
                {
                    parent = current;
                    current = current.LeftNode;
                }
                else if (result > 0)
                {
                    parent = current;
                    current = current.RightNode;
                }
                if (current == null)
                    return false;
            }
            while (result != 0);

            if (current.RightNode == null)
            {
                if (current == RootNode)
                    RootNode = current.LeftNode;
                else
                {
                    result = Comparer.Compare(current.Data, parent.Data);
                    if (result < 0)
                        parent.LeftNode = current.LeftNode;
                    else
                        parent.RightNode = current.LeftNode;
                }
            }
            else if (current.RightNode.LeftNode == null)
            {
                current.RightNode.LeftNode = current.LeftNode;
                if (current == RootNode)
                    RootNode = current.RightNode;
                else
                {
                    result = Comparer.Compare(current.Data, parent.Data);
                    if (result < 0)
                        parent.LeftNode = current.RightNode;
                    else
                        parent.RightNode = current.RightNode;
                }
            }
            else
            {
                BinaryTreeNode<T> min = current.RightNode.LeftNode, prev = current.RightNode;
                while (min.LeftNode != null)
                {
                    prev = min;
                    min = min.LeftNode;
                }
                prev.RightNode = min.RightNode;
                min.RightNode = current.RightNode;
                min.RightNode = current.RightNode;

                if (current == RootNode)
                    RootNode = min;
                else
                {
                    result = Comparer.Compare(current.Data, parent.Data);
                    if (result < 0)
                        parent.LeftNode = min;
                    else
                        parent.RightNode = min;
                }
            }
            --Count;
            return true;
        }

        public void Clear()
        {
            RootNode = null;
            Count = 0;
        }

        public bool Contains(T item)
        {
            return FindNode(item) is not null;
        }

        public T? Find(T item)
        {
            var itemNode = FindNode(item);
            if (itemNode != null)
                return itemNode.Data;
            return default(T);
        }

        /// <summary>
        /// Поиск узла по значению
        /// </summary>
        /// <param name="data">Искомое значение</param>
        /// <param name="startWithNode">Узел начала поиска</param>
        /// <returns>Найденный узел</returns>
        public BinaryTreeNode<T>? FindNode(T data, BinaryTreeNode<T>? startWithNode = null)
        {
            startWithNode ??= RootNode;

            if (startWithNode == null)
                return null;

            int result = Comparer.Compare(data, startWithNode.Data);

            if (result == 0)
            {
                return startWithNode;
            }
            else if (result < 0)
            {
                if (startWithNode.LeftNode == null)
                {
                    return null;
                }
                else
                {
                    return FindNode(data, startWithNode.LeftNode);
                }
            }
            else
            {
                if (startWithNode.RightNode == null)
                {
                    return null;
                }
                else
                {
                    return FindNode(data, startWithNode.RightNode);
                }
            }

        }

        public void CopyTo(T[] array, int arrayIndex)
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
            return (object) new BinaryTree<T>(this);
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
            if(Count == 0)
                Console.WriteLine("Дерево пустое");
            else
                PrintTree(RootNode);
        }


        /// <summary>
        /// Вывод бинарного дерева начиная с указанного узла
        /// </summary>
        /// <param name="startNode">Узел с которого начинается печать</param>
        /// <param name="indent">Отступ</param>
        /// <param name="side">Сторона</param>
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