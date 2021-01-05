using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeWinForms
{
    public class MainForm : Form
    {
        Model model;
        TreeView treeView;
        Button addChildButton;
        Button removeNodeButton;
        Label label1;
        Label label2;
        Label label3;
        TextBox nameTextBox;
        TextBox descriptionTextBox;
        MenuItem saveMenuItem;
        MenuItem openMenuItem;
        MenuItem menuItem;
        private void CreateControls()
        {
            treeView = new TreeView()
            {
                Dock = DockStyle.Fill
            };

            addChildButton = new Button
            {
                Text = "Add child",
                Dock = DockStyle.Fill
            };

            removeNodeButton = new Button
            {
                Text = "Remove node",
                Dock = DockStyle.Fill
            };

            label1 = new Label
            {
                Text = "Properties:",
                Dock = DockStyle.Fill
            };

            label2 = new Label
            {
                Text = "Name:",
                Dock = DockStyle.Fill
            };

            label3 = new Label
            {
                Text = "Description:",
                Dock = DockStyle.Fill
            };

            nameTextBox = new TextBox
            {
                Dock = DockStyle.Fill
            };

            descriptionTextBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill
            };
        }
        private TableLayoutPanel CreateLayouts()
        {
            var nameTextLayout = new TableLayoutPanel();
            nameTextLayout.ColumnStyles.Clear();
            nameTextLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            nameTextLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            nameTextLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            nameTextLayout.Controls.Add(label2, 0, 0);
            nameTextLayout.Controls.Add(nameTextBox, 1, 0);
            nameTextLayout.Dock = DockStyle.Fill;

            var descriptionTextLayout = new TableLayoutPanel();
            descriptionTextLayout.RowStyles.Clear();
            descriptionTextLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            descriptionTextLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            descriptionTextLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            descriptionTextLayout.Controls.Add(label3, 0, 0);
            descriptionTextLayout.Controls.Add(descriptionTextBox, 0, 1);
            descriptionTextLayout.Dock = DockStyle.Fill;

            var buttonLayout = new TableLayoutPanel();
            buttonLayout.ColumnStyles.Clear();
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            buttonLayout.Controls.Add(addChildButton, 0, 0);
            buttonLayout.Controls.Add(removeNodeButton, 1, 0);
            buttonLayout.Dock = DockStyle.Fill;

            var leftLayout = new TableLayoutPanel();
            leftLayout.RowStyles.Clear();
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            leftLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            leftLayout.Controls.Add(treeView, 0, 0);
            leftLayout.Controls.Add(buttonLayout, 0, 1);
            leftLayout.Dock = DockStyle.Fill;

            var rightLayout = new TableLayoutPanel();
            rightLayout.RowStyles.Clear();
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            rightLayout.Controls.Add(label1, 0, 0);
            rightLayout.Controls.Add(nameTextLayout, 0, 1);
            rightLayout.Controls.Add(descriptionTextLayout, 0, 2);
            rightLayout.Controls.Add(new Panel(), 0, 3);
            rightLayout.Dock = DockStyle.Fill;

            var mainLayout = new TableLayoutPanel();
            mainLayout.ColumnStyles.Clear();
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.Controls.Add(leftLayout, 0, 0);
            mainLayout.Controls.Add(rightLayout, 1, 0);
            mainLayout.Dock = DockStyle.Fill;

            return mainLayout;
        }

        private MainMenu CreateMenu()
        {
            saveMenuItem = new MenuItem("Save Tree");
            openMenuItem = new MenuItem("Open Tree");
            menuItem = new MenuItem("Menu",
                                    new[]
                                    {
                                        saveMenuItem,
                                        openMenuItem
                                    }
                                    );
            var mainMenu = new MainMenu(
                                    new[]
                                    {
                                        menuItem
                                    }
                                    );
            return mainMenu;
        }

        public MainForm(Model tModel, Controller controller)
        {
            model = tModel;
            this.Size = new Size(600, 480);

            CreateControls();
            Controls.Add(CreateLayouts());

            this.Menu = CreateMenu();

            treeView.Nodes.Add("MainNode","MainNode");
            addChildButton.Click+= (sender, args) => controller.AddChild(treeView.SelectedNode.FullPath);
            removeNodeButton.Click += (sender, args) => controller.RemoveNode(treeView.SelectedNode.FullPath);

            treeView.AfterSelect += (sender, args) =>
            {
                List<string> parameters = controller.GetNodeData(treeView.SelectedNode.FullPath);
                nameTextBox.Text = parameters[0];
                descriptionTextBox.Text = parameters[1];
            };

            treeView.BeforeSelect += (sender, args) =>
            {
                if (treeView.SelectedNode != null)
                {
                    controller.UpdateNode(
                                      treeView.SelectedNode.FullPath,
                                      nameTextBox.Text,
                                      descriptionTextBox.Text);
                }                
            };

            saveMenuItem.Click+= (sender, args) => controller.SaveTree();
  
            openMenuItem.Click += (sender, args) =>
            {
                string s = OpenJSONFile();
                if (s != null)
                    UploadTree(controller,s);
                    //controller.UploadTree(s);
            };

            model.TreeElementAdded += OnTreeAdded;
            model.TreeElementRemoved += OnTreeRemoved;
            model.TreeElementUpdated += OnTreeUpdated;
            model.TreeSaved += OnSaveJSONData;
            model.TreeUploaded += OnTreeUploaded;
        }

        private string OpenJSONFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog.FileName;

                string s = System.IO.File.ReadAllText(filename);

                MessageBox.Show("File has been opened");
                return s;
            }
            return null;
        }

        private void UploadTree(Controller controller, string s)
        {
            controller.UploadTree(s);
            List<string> rootNodeNames = controller.GetRootData();

            var stack = new Stack<(string,string)>();

            foreach (var name in rootNodeNames)
            {
                stack.Push((name, name));
                treeView.Nodes.Add(name, name);
            }
            while (stack.Count != 0)
            {
                var currentNode = stack.Pop();
                var name = currentNode.Item1;
                var path = currentNode.Item2;
                var children = controller.GetNodesChildren(path);

                if (children!=null)
                {
                    foreach (var child in children)
                    {
                        stack.Push((name, path+'\\'+child));
                        AddItemToTree(path.Split('\\').ToList(), child);
                    }
                }
            }

        }
        private void OnTreeUploaded()
        {
            /*foreach (var element in elements)
            {
                AddItemToTree(element.Item1, element.Item2);
            }*/

            /*var stack = new Stack<TreeNode<TKey, TValue>>();

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
            }*/
            treeView.Nodes.Clear();
            /*var stack = new Stack<(string,string)>();
            foreach (var element in elements)
                stack.Push(element);
            while (stack.Count!=0)
            {
                var currentNode = stack.Pop();
                var name = currentNode.Item1;
                //var description = currentNode.Item2;
                treeView.Nodes.Add(name, name);

                var children = controller
                foreach ()
            }*/
        }

        private void OnSaveJSONData(string s)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;

                System.IO.File.WriteAllText(filename, s);
                
                MessageBox.Show("File has been saved");
            }
        }

        private void OnTreeUpdated(List<string> path, string name, bool canBeUpdated)
        {
            if (canBeUpdated)
            {
                List<string> pathWithoutName = path.Take(path.Count - 1).ToList();
                TreeNodeCollection treeNodeCollection = FindTreeNodeCollection(pathWithoutName);
                TreeNode lastTreeNode = treeNodeCollection.Find(path.Last(), false).FirstOrDefault();
                lastTreeNode.Name = name;
                lastTreeNode.Text = name;
            }
        }

        private void OnTreeRemoved(List<string> path, bool canDelete)
        {
            if (!canDelete)
            {
                MessageBox.Show("The Item cannot be deleted");
            }
            else
            {
                treeView.BeginUpdate();

                List<string> pathWithoutName = path.Take(path.Count - 1).ToList();

                TreeNodeCollection treeNodeCollection = FindTreeNodeCollection(pathWithoutName);

                TreeNode lastTreeNode = treeNodeCollection.Find(path.Last(), false).FirstOrDefault();
                treeNodeCollection.Remove(lastTreeNode);

                treeView.EndUpdate();
            }           
        }

        private void OnTreeAdded(List<string> path, string name)
        {
            AddItemToTree(path, name);            
        }

        private void AddItemToTree(List<string> path, string name)
        {
            treeView.BeginUpdate();

            TreeNodeCollection treeNodeCollection = FindTreeNodeCollection(path);
            treeNodeCollection.Add(name, name);

            treeView.EndUpdate();
        }

        private TreeNodeCollection FindTreeNodeCollection(List<string> path)
        {
            TreeNodeCollection treeNodeCollection = treeView.Nodes;

            foreach (var s in path)
            {
                TreeNode newTreeNode = treeNodeCollection.Find(s, false).FirstOrDefault();
                treeNodeCollection = newTreeNode.Nodes;
            }

            return treeNodeCollection;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            model.TreeElementAdded -= OnTreeAdded;
            model.TreeElementRemoved -= OnTreeRemoved;
        }
        private void TestMethod3()
        {
            var jsonUser = JsonConvert.SerializeObject(this);
            //System.Console.Write(jsonUser);
            MessageBox.Show(jsonUser);           
        }
        private void TestMethod2()
        {
            MessageBox.Show(treeView.SelectedNode.FullPath);
            if (treeView.SelectedNode != null)
            {
                treeView.Nodes.Remove(treeView.SelectedNode);
            }
        }

        private void TestMethod1()
        {           
                treeView.BeginUpdate();
                treeView.SelectedNode.Nodes.Add("newChildNode");
                treeView.EndUpdate();
        }
    }
}