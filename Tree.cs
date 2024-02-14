﻿using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;

namespace Trees
{
    /// <summary>
    /// Расположения узла относительно родителя
    /// </summary>
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

        public bool Remove(T data)
        {
            var foundNode = FindNode(data);
            //Remove(foundNode);

            if (foundNode == null)
                return false;

            Count--;
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

            int index = arrayIndex;
            foreach (var item in this)
            {
                array[index++] = item;
            }
        }

        #endregion

        #region IEnumerable<T> members
        public IEnumerator<T> GetEnumerator()
        {
            return InOrderTraversal(RootNode).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private IEnumerable<T> InOrderTraversal(BinaryTreeNode<T>? node)
        {
            if (node != null)
            {
                foreach (var item in InOrderTraversal(node.LeftNode))
                {
                    yield return item;
                }

                yield return node.Data;

                foreach (var item in InOrderTraversal(node.RightNode))
                {
                    yield return item;
                }
            }
        }

        #endregion

        #region ICloneable members
        public object ShallowCopy()
        {
            var newTree = new BinaryTree<T>(this.Comparer)
            {
                RootNode = this.RootNode,
                Count = this.Count
            };
            return newTree;
        }

        public object Clone()
        {
            var clonedTree = new BinaryTree<T>(this.Comparer)
            {
                Count = this.Count
            };

            if(Count != 0)
                clonedTree.RootNode = CloneNode(this.RootNode);
            return clonedTree;
        }

        private BinaryTreeNode<T>? CloneNode(BinaryTreeNode<T>? node)
        {
            if (node == null)
                return null;


            var clonedNode = new BinaryTreeNode<T>(node.Data)
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