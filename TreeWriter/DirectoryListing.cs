using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TreeWriterWF
{
    public partial class DirectoryListing : ControllerPanel
    {
        public class NodeTag
        {
            public enum Type
            {
                Directory,
                File,
                Root
            }

            public Type NodeType;
            public String Path;
        }

        private TreeNode ContextNode = null;
        private Project Project;

        // Todo: Watch the filesystem for changes and automatically update directory tree.
        // https://msdn.microsoft.com/en-us/library/system.io.filesystemwatcher.aspx

        public DirectoryListing(Project Project)
        {
            this.Project = Project;

            InitializeComponent();

            var directoryPath = System.IO.Path.GetDirectoryName(Project.Path);
            BuildDirectoryTreeItems(directoryPath, treeView.Nodes);
            Text = System.IO.Path.GetFileName(Project.Path);
        }

        private TreeNode BuildDirectoryTree(String DirectoryPath)
        {
            var r = new TreeNode()
            {
                Text = System.IO.Path.GetFileName(DirectoryPath),
                Tag = new NodeTag
                {
                    NodeType = NodeTag.Type.Directory,
                    Path = DirectoryPath
                },
                ImageIndex = 1,
                SelectedImageIndex = 1,
            };
            BuildDirectoryTreeItems(DirectoryPath, r.Nodes);
            return r;
        }

        private void BuildDirectoryTreeItems(String DirectoryPath, TreeNodeCollection Into)
        {
            foreach (var subDir in System.IO.Directory.EnumerateDirectories(DirectoryPath))
                Into.Add(BuildDirectoryTree(subDir));
            foreach (var file in System.IO.Directory.EnumerateFiles(DirectoryPath))
            {
                var fileName = System.IO.Path.GetFileName(file);
                var extension = System.IO.Path.GetExtension(file);
                if (extension != ".txt") continue;
                var fileNode = new TreeNode() { Text = System.IO.Path.GetFileNameWithoutExtension(file) };
                fileNode.Tag = new NodeTag { NodeType = NodeTag.Type.File, Path = file };
                fileNode.ImageIndex = 0;
                fileNode.SelectedImageIndex = 0;
                Into.Add(fileNode);
            }
        }

        public void UpdateNode(TreeNode Node)
        {
            if (Node == null)
            {
                treeView.Nodes.Clear();
                BuildDirectoryTreeItems(System.IO.Path.GetDirectoryName(Project.Path), treeView.Nodes);
            }
            else
            {
                Node.Nodes.Clear();
                BuildDirectoryTreeItems((Node.Tag as NodeTag).Path, Node.Nodes);
            }
        }            

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var file = e.Node.Tag as NodeTag;
            if (file.NodeType == NodeTag.Type.File)
                ControllerCommand(new Commands.OpenDocument(file.Path, Project));
        }

        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = treeView.GetNodeAt(p);
                if (node != null && node.Tag != null)
                {
                    ContextNode = node;
                    treeView.SelectedNode = ContextNode;

                    var tag = node.Tag as NodeTag;
                    if (tag.NodeType == NodeTag.Type.Directory)
                        DirectoryContextMenu.Show(treeView, p);
                    else if (tag.NodeType == NodeTag.Type.File)
                        FileContextMenu.Show(treeView, p);
                }
                else if (node == null)
                {
                    ContextNode = null;
                    DirectoryContextMenu.Show(treeView, p);
                }
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            if (e.Node.Tag == null)
            {
                e.CancelEdit = true;
                return;
            }

            var tag = e.Node.Tag as NodeTag;

            if (e.Label == System.IO.Path.GetFileNameWithoutExtension(tag.Path)) return;

            if (tag.NodeType == NodeTag.Type.File)
            {
                var directory = System.IO.Path.GetDirectoryName(tag.Path);
                var newPath = directory + "\\" + e.Label + ".txt";

                ControllerCommand(new Commands.RenameDocument(tag.Path, newPath));

                tag.Path = newPath;
            }
            else if (tag.NodeType == NodeTag.Type.Directory)
            {
                var directory = System.IO.Path.GetDirectoryName(tag.Path);
                var newPath = directory + "\\" + e.Label;

                ControllerCommand(new Commands.RenameFolder(tag.Path, newPath));

                tag.Path = newPath;
                UpdateNode(e.Node);
            }
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.CreateNewDocument createCommand = null;
            TreeNode newNode = null;
            
            if (ContextNode != null)
            {
                var tag = ContextNode.Tag as NodeTag;
                createCommand = new Commands.CreateNewDocument(tag.Path);
                ControllerCommand(createCommand);
                UpdateNode(ContextNode);
                foreach (TreeNode node in ContextNode.Nodes)
                    if ((node.Tag as NodeTag).Path == tag.Path + "\\" + createCommand.NewFileName)
                        newNode = node;
            }
            else
            {
                createCommand = new Commands.CreateNewDocument(System.IO.Path.GetDirectoryName(Project.Path));
                ControllerCommand(createCommand);
                UpdateNode(null);
                foreach (TreeNode node in treeView.Nodes)
                    if ((node.Tag as NodeTag).Path == System.IO.Path.GetDirectoryName(Project.Path) + "\\" + createCommand.NewFileName)
                        newNode = node;
            }

            newNode.EnsureVisible();
            treeView.SelectedNode = newNode;
            newNode.BeginEdit();
        }

        private void deleteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ContextNode == null) return;
            var tag = ContextNode.Tag as NodeTag;
            ControllerCommand(new Commands.DeleteFolder(tag.Path));
            UpdateNode(ContextNode.Parent);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ContextNode == null) return;
            System.Diagnostics.Debug.Assert(ContextNode != null && (ContextNode.Tag as NodeTag).NodeType == NodeTag.Type.File);
            var tag = ContextNode.Tag as NodeTag;
            ControllerCommand(new Commands.DeleteDocument(tag.Path));
            UpdateNode(ContextNode.Parent);
        }

        private void DirectoryListing_FormClosing(object sender, FormClosingEventArgs e)
        {
            ControllerCommand(new Commands.CloseProject(Project));
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Commands.CreateFolder createCommand = null;
            TreeNode newNode = null;

            if (ContextNode != null)
            {
                var tag = ContextNode.Tag as NodeTag;
                createCommand = new Commands.CreateFolder(tag.Path);
                ControllerCommand(createCommand);
                UpdateNode(ContextNode);
                foreach (TreeNode node in ContextNode.Nodes)
                    if ((node.Tag as NodeTag).Path == tag.Path + "\\" + createCommand.NewFileName)
                        newNode = node;
            }
            else
            {
                createCommand = new Commands.CreateFolder(System.IO.Path.GetDirectoryName(Project.Path));
                ControllerCommand(createCommand);
                UpdateNode(null);
                foreach (TreeNode node in treeView.Nodes)
                    if ((node.Tag as NodeTag).Path == System.IO.Path.GetDirectoryName(Project.Path) + "\\" + createCommand.NewFileName)
                        newNode = node;
            }

            newNode.EnsureVisible();
            treeView.SelectedNode = newNode;
            newNode.BeginEdit();
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (treeView.SelectedNode != null)
                {
                    var tag = treeView.SelectedNode.Tag as NodeTag;
                    if (tag != null && tag.NodeType == NodeTag.Type.File)
                        ControllerCommand(new Commands.OpenDocument(tag.Path, Project));
                    else if (tag != null)
                        treeView.SelectedNode.Toggle();
                }
            }
        }
    }
}
