using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWinForms
{
    public class Tree<TKey,TValue>
        where TKey : IComparable
        where TValue : new()
    {
        private TreeNode<TKey,TValue> root;

        public Tree()
        {
            root = new TreeNode<TKey,TValue>(default(TKey), default(TValue), null, 0);
        }

        /*public IEnumerable<TreeNode<TKey,TValue>> DepthSearch()
        {
            var stack = new Stack<TreeNode<TKey, TValue>>();

            foreach (var element in root.Children)
                stack.Push(element);
            while (stack.Count != 0)
            {
                var currentNode = stack.Pop();
                yield return currentNode;
                if (currentNode.Children != null)
                {
                    foreach (var child in currentNode.Children)
                        stack.Push(child);
                }
            }
        }*/
        public IEnumerable<TValue> DepthSearch()
        {
            var stack = new Stack<TreeNode<TKey, TValue>>();

            foreach (var element in root.Children)
                stack.Push(element);
            while (stack.Count != 0)
            {
                var currentNode = stack.Pop();
                yield return currentNode.Value;
                if (currentNode.Children != null)
                {
                    foreach (var child in currentNode.Children)
                        stack.Push(child);
                }
            }
        }

        public List<TValue> GetRootElements()
        {
            return root.Children.Select(x => x.Value).ToList();
        }

        public string GetJSON()
        {           
            return JsonConvert.SerializeObject(root.Children, Formatting.Indented);
        }

        public void DeserializeJSON(string s)
        {
            var nodes = JsonConvert.DeserializeObject<TreeNode<TKey, TValue>[]>(s);
            root.Children = nodes.ToList();
        }

        public TValue GetNode(List<TKey> path)
        {
            return GetTreeNode(path).Value;
        }

        public void AddNode(List<TKey> path, TKey key, TValue child)
        {
            TreeNode<TKey, TValue> parent = GetTreeNode(path);

            var newNode = new TreeNode<TKey, TValue>(key, child, null, parent.Depth + 1);

            if (parent.Children==null)
            {
                parent.Children = new List<TreeNode<TKey, TValue>>();
            }
            parent.Children.Add(newNode);
        }

        public List<TValue> GetAllChildren(List<TKey> path)
        {
            if (GetTreeNode(path).Children != null)
                return GetTreeNode(path).Children.Select(x => x.Value).ToList();
            else
                return null;
        }

        public void RemoveNode(List<TKey> path)
        {
            TKey keyName = path.Last();
            TreeNode<TKey, TValue> node = GetTreeNode(path.Take(path.Count-1).ToList());
            TreeNode<TKey, TValue> nodeToRemove = node.Children
                                                      .Where(x => x.Key.CompareTo(keyName) == 0).FirstOrDefault();
            node.Children.Remove(nodeToRemove);
        }

        public bool UpdateNode(List<TKey> path,TKey key, TValue value)
        {
            TreeNode<TKey, TValue> node = GetTreeNode(path);
            node.Value = value;
            if (KeyIsValid(path,key))
            {              
                node.Key = key;
                return true;
            }
            return false;
        }
        private bool KeyIsValid(List<TKey> path, TKey key)
        {
            TreeNode<TKey, TValue> parentNode = GetTreeNode(path.Take(path.Count - 1).ToList());
            if (parentNode.Children.Where(x => x.Key.CompareTo(key) == 0).Count() == 0)
                return true;
            else
                return false;
        }
        private TreeNode<TKey, TValue> GetTreeNode(List<TKey> path)
        {
            if (path == null) return root;
            var currentNode = root;

            foreach (var element in path)
            {
                currentNode = currentNode.Children.Where(x => x.Key.CompareTo(element) == 0).FirstOrDefault();
                if (currentNode == null) throw new Exception("Wrong path");
            }
            return currentNode;
        }
    }

    public class TreeNode<TKey,TValue>
        where TKey : IComparable
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public int Depth { get; set; }
        public List<TreeNode<TKey,TValue>> Children { get; set; }

        //public TreeNode() { }

        public TreeNode(TKey key, TValue value, List<TreeNode<TKey, TValue>> children, int depth)
        {
            Key = key; 
            Value = value;
            Children = children;
            Depth = depth;
        }

        public int CompareTo(TreeNode<TKey, TValue> element)
        {
            return (-1) * Key.CompareTo(element.Key);
        }
    }
}
