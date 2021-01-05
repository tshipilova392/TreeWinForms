using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWinForms
{
    public class Controller
    {
        private Model model;
        public Controller(Model tModel)
        {
            model = tModel;
        }
        /*public void Flip(int row, int column)
        {
            for (int iRow = 0; iRow < gameModel.Size; iRow++)
                if (iRow != row) gameModel.FlipState(iRow, column);
            for (int iColumn = 0; iColumn < gameModel.Size; iColumn++)
                if (iColumn != column) gameModel.FlipState(row, iColumn);
            gameModel.FlipState(row, column);
        }*/
        public void AddChild(string path)
        {
            // List<Node> nodesList = path.Split('\\').Select(x=>new Node(x)).ToList();
            // nodesList.Add(new Node("newNode"));
            List<string> pathList = path.Split('\\').ToList();
            model.AddNewElement(pathList);
        }

        public void RemoveNode(string path)
        {
            List<string> pathList = path.Split('\\').ToList();           
            model.RemoveElement(pathList);                      
        }

        public void UpdateNode(string path,string name,string description)
        {
            List<string> pathList = path.Split('\\').ToList();
            model.UpdateElement(pathList,name, new Node(name, description));
        }

        public List<string> GetNodeData(string path)
        {
            List<string> pathList = path.Split('\\').ToList();
            Node data = model.GetData(pathList);
            List<string> list = new List<string>();
            list.Add(data.Name);
            list.Add(data.Description);
            return list;
        }

        public List<string> GetRootData()
        {
            List<Node> nodes = model.GetRootData();
            List<string> namesList = new List<string>();
            
            foreach (var node in nodes)
            {
                namesList.Add(node.Name);
            }

            return namesList;
        }

        public List<string> GetNodesChildren(string path)
        {
            List<string> pathList = path.Split('\\').ToList();
            List<string> childrenNames = model.GetChildrenNames(pathList);

            return childrenNames;
        }

        public void SaveTree()
        {
            model.SaveData();
        }

        public void UploadTree(string s)
        {
            model.UploadData(s);
        }
    }
}
