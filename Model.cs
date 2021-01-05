using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWinForms
{
    public class Model
    {
        Tree<string,Node> tree;

        public Model()
        {
            tree = new Tree<string, Node>();
        }

        public void Start()
        {
			Node rootNode = new Node("MainNode");
			tree.AddNode(null, "MainNode", rootNode);
		}

		public void AddNewElement(List<string> path)
		{
			List<Node> childrenNodes = tree.GetAllChildren(path);
			string newName;
			if (childrenNodes==null)
			{
				newName = "NewName";
			}
			else
			{
				List<string> childrenNames = childrenNodes.Select(x => x.Name).ToList();
				newName = ChooseNewName(childrenNames);			
			}
			tree.AddNode(path, newName, new Node(newName));

			if (TreeElementAdded != null)
				TreeElementAdded.Invoke(path,newName);
		}

		private string ChooseNewName(List<string> names)
		{
			string name = "NewName";
			StringBuilder tmpName = new StringBuilder();

			for (int i=1; ;i++)
			{
				tmpName.Clear();
				tmpName.Append(name);
				tmpName.Append(i);
				if (!names.Contains(tmpName.ToString())) break;
			}

			return tmpName.ToString();
		}

		public void RemoveElement(List<string> path)
		{
			bool canDelete=false;
			if (path.Count!=1)
			{
				canDelete = true;
				tree.RemoveNode(path);
			}			
			if (TreeElementRemoved!=null)
				TreeElementRemoved.Invoke(path,canDelete);
		}

		public void UpdateElement(List<string> names, string newName, Node node)
		{
			bool result = tree.UpdateNode(names, newName, node);
			if (TreeElementUpdated != null)
				TreeElementUpdated.Invoke(names, newName, result);
		}

		public Node GetData(List<string> path)
		{
			return tree.GetNode(path);
		}

		public List<Node> GetRootData()
		{
			return tree.GetRootElements();
		}

		public List<string> GetChildrenNames(List<string> path)
		{
			var childrenNodes = tree.GetAllChildren(path);
			if (childrenNodes!=null)
				return childrenNodes.Select(x=>x.Name).ToList();
			return null;
		}

		public void SaveData()
		{
			var s = tree.GetJSON();

			if (TreeSaved != null)
				TreeSaved.Invoke(s);
		}

		public void UploadData(string s)
		{
			tree.DeserializeJSON(s);
			/*var treeItems = tree.DepthSearch();
			var listOfElements = new List<(List<string>, string)>();

			foreach (var item in treeItems)
			{
				item.
				listOfElements.Add(Tuple.Create());
			}*/
			List<Node> rootElements = tree.GetRootElements();
			if (TreeUploaded != null)
				TreeUploaded.Invoke();
			//TreeUploaded.Invoke(rootElements.Select(x=>(x.Name,x.Description)).ToList());
		}

		public event Action<List<string>,string> TreeElementAdded;
		public event Action<List<string>,bool> TreeElementRemoved;
		public event Action<List<string>, string, bool> TreeElementUpdated;
		public event Action<string> TreeSaved;
		public event Action TreeUploaded;
	}

	public class Node 
	{
		public string Name { get; set; }
		public string Description { get; set; }

		public Node()
		{

		}
		public Node(string name)
		{
			Name = name;
			Description = "";
		}

		public Node(string name,string description)
		{
			Name = name;
			Description = description;
		}
	}
}
