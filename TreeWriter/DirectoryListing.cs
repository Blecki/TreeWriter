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
        private String DirectoryPath;

        // Todo: Watch the filesystem for changes and automatically update directory tree.
        // https://msdn.microsoft.com/en-us/library/system.io.filesystemwatcher.aspx

        public DirectoryListing(String Path)
        {
            this.DirectoryPath = Path;

            InitializeComponent();

            var root = BuildDirectoryTree(Path);
            (root.Tag as NodeTag).NodeType = NodeTag.Type.Root;
            treeView.Nodes.Add(root);
            Text = System.IO.Path.GetFileName(Path);
            root.Expand();
        }

        private TreeNode BuildDirectoryTree(String DirectoryPath)
        {
            var r = new TreeNode() { Text = System.IO.Path.GetFileName(DirectoryPath), Tag = new NodeTag {
            NodeType = NodeTag.Type.Directory,
            Path = DirectoryPath }
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
                Into.Add(fileNode);
            }
        }

        private void UpdateNode(TreeNode Node)
        {
            Node.Nodes.Clear();
            BuildDirectoryTreeItems((Node.Tag as NodeTag).Path, Node.Nodes);
        }            

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var file = e.Node.Tag as NodeTag;
            if (file.NodeType == NodeTag.Type.File)
                ControllerCommand(new Commands.OpenFile(file.Path));
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
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag == null)
            {
                e.CancelEdit = true;
                return;
            }

            var tag = e.Node.Tag as NodeTag;
            var directory = System.IO.Path.GetDirectoryName(tag.Path);
            var newPath = directory + "\\" + e.Label + ".txt";

            ControllerCommand(new Commands.RenameFile(tag.Path, newPath));

            tag.Path = newPath;
            if (tag.NodeType == NodeTag.Type.Directory)
                UpdateNode(e.Node);
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ContextNode != null && (ContextNode.Tag as NodeTag).NodeType == NodeTag.Type.Directory);
            var tag = ContextNode.Tag as NodeTag;
            var command = new Commands.CreateFile(tag.Path);
            ControllerCommand(command);
            UpdateNode(ContextNode);
            // TODO: Select the newly created file node.
        }

        private void deleteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ContextNode != null && (ContextNode.Tag as NodeTag).NodeType == NodeTag.Type.Directory);
            var tag = ContextNode.Tag as NodeTag;
            ControllerCommand(new Commands.DeleteFolder(tag.Path));
            UpdateNode(ContextNode.Parent);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(ContextNode != null && (ContextNode.Tag as NodeTag).NodeType == NodeTag.Type.File);
            var tag = ContextNode.Tag as NodeTag;
            ControllerCommand(new Commands.DeleteFile(tag.Path));
            UpdateNode(ContextNode.Parent);
        }

        private void DirectoryListing_FormClosing(object sender, FormClosingEventArgs e)
        {
            ControllerCommand(new Commands.CloseDirectory(DirectoryPath));
        }
    }
}
