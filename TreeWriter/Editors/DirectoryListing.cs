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
    public partial class DirectoryListing : DocumentEditor
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
            public int WordCount;
        }

        private TreeNode ContextNode = null;
        private FolderDocument Project;

        // Todo: Watch the filesystem for changes and automatically update directory tree.
        // https://msdn.microsoft.com/en-us/library/system.io.filesystemwatcher.aspx

        public DirectoryListing(FolderDocument Project) : base(Project)
        {
            this.Project = Project;

            InitializeComponent();

            var directoryPath = Project.Path;
            BuildDirectoryTreeItems(directoryPath, treeView.Nodes, new MetaInformation(Project.Path));
            Text = System.IO.Path.GetFileName(Project.Path);

            var refreshMenuItem = new ToolStripMenuItem("Refresh");
            refreshMenuItem.Click += refreshMenuItem_Click;
            this.contextMenuStrip1.Items.Add(refreshMenuItem);

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            var setStyleMethod = typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (setStyleMethod != null) setStyleMethod.Invoke(treeView,
                new Object[] {
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, 
                true});
        }

        void refreshMenuItem_Click(object sender, EventArgs e)
        {
            ReloadDocument();
        }

        private TreeNode BuildDirectoryTree(String DirectoryPath)
        {
            var meta = new MetaInformation(DirectoryPath);

            var r = new TreeNode()
            {
                Text = System.IO.Path.GetFileName(DirectoryPath),
                Tag = new NodeTag
                {
                    NodeType = NodeTag.Type.Directory,
                    Path = DirectoryPath,
                    WordCount = meta.Data.TotalWordCount
                },
                ImageIndex = 1,
                SelectedImageIndex = 1,
            };
         
            BuildDirectoryTreeItems(DirectoryPath, r.Nodes, meta);
            return r;
        }

        private void BuildDirectoryTreeItems(String DirectoryPath, TreeNodeCollection Into, MetaInformation Meta)
        {
            foreach (var subDir in Model.EnumerateDirectories(DirectoryPath))
            {
                var dirNode = BuildDirectoryTree(subDir);
                Into.Add(dirNode);
            }
        
            foreach (var file in System.IO.Directory.EnumerateFiles(DirectoryPath))
            {
                var fileName = System.IO.Path.GetFileName(file);
                var extension = System.IO.Path.GetExtension(file);
                if (extension == ".txt")
                {
                    var fileNode = new TreeNode() { Text = System.IO.Path.GetFileName(file) };
                    fileNode.Tag = new NodeTag { NodeType = NodeTag.Type.File, Path = file };
                    fileNode.ImageIndex = 0;
                    fileNode.SelectedImageIndex = 0;
                    Into.Add(fileNode);

                    if (Meta.Data.Files.ContainsKey(file))
                        (fileNode.Tag as NodeTag).WordCount = Meta.Data.Files[file].WordCount;
                }
            }
        }

        public void UpdateNode(TreeNode Node)
        {
            if (Node == null)
            {
                treeView.Nodes.Clear();
                BuildDirectoryTreeItems(Project.Path, treeView.Nodes, new MetaInformation(Project.Path));
            }
            else
            {
                Node.Nodes.Clear();
                BuildDirectoryTreeItems((Node.Tag as NodeTag).Path, Node.Nodes, new MetaInformation((Node.Tag as NodeTag).Path));
            }
        }

        public override void ReloadDocument()
        {
            UpdateNode(null);
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var file = e.Node.Tag as NodeTag;
            if (file.NodeType == NodeTag.Type.File)
            {
                try
                {
                    InvokeCommand(new Commands.OpenPath(file.Path, Commands.OpenCommand.OpenStyles.CreateView));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                }
            }
        }

        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                TreeNode node = treeView.GetNodeAt(p);

                var menu = new ContextMenu();
                ContextNode = node;
                if (ContextNode != null) treeView.SelectedNode = ContextNode;

                menu.MenuItems.Add("New File", NewFileMenuItemHandler);
                menu.MenuItems.Add("New Folder", NewFolderMenuItemHandler);

                if (ContextNode != null)
                {
                    menu.MenuItems.Add("Refresh Wordcount", RefreshWordCountMenuItemHandler);
                    menu.MenuItems.Add("Notes", (_s, args) =>
                        {
                            InvokeCommand(new Commands.OpenPath((ContextNode.Tag as NodeTag).Path + ".$notes", Commands.OpenCommand.OpenStyles.CreateView));
                        });
                }

                if (ContextNode == null || (ContextNode.Tag as NodeTag).NodeType == NodeTag.Type.Directory)
                {
                    if (ContextNode != null)
                        menu.MenuItems.Add("Delete", (_s, args) =>
                        {
                            if (!Confirm("Are you sure you want to delete this folder?")) return;
                            if (ContextNode == null) return;
                            var tag = ContextNode.Tag as NodeTag;
                            InvokeCommand(new Commands.DeleteFolder(tag.Path));
                            UpdateNode(ContextNode.Parent);
                        });
                }
                else
                {
                    menu.MenuItems.Add("Delete", (_s, args) =>
                        {
                            if (!Confirm("Are you sure you want to delete this document?")) return;
                            if (ContextNode == null) return;
                            System.Diagnostics.Debug.Assert(ContextNode != null && (ContextNode.Tag as NodeTag).NodeType == NodeTag.Type.File);
                            var tag = ContextNode.Tag as NodeTag;
                            InvokeCommand(new Commands.DeleteDocument(tag.Path));
                            UpdateNode(ContextNode.Parent);
                        });

                }

                menu.Show(treeView, p);
            }
        }

        private void NewFileMenuItemHandler(object sender, EventArgs e)
        {
            Commands.CreateNewDocument createCommand = null;
            TreeNode newNode = null;

            if (ContextNode != null)
            {
                var tag = ContextNode.Tag as NodeTag;
                var dirPath = tag.Path;
                if (tag.NodeType == NodeTag.Type.File)
                    dirPath = System.IO.Path.GetDirectoryName(dirPath);
                createCommand = new Commands.CreateNewDocument(dirPath, "txt");
                InvokeCommand(createCommand);
                UpdateNode(ContextNode);
                foreach (TreeNode node in ContextNode.Nodes)
                    if ((node.Tag as NodeTag).Path == dirPath + "\\" + createCommand.NewFileName)
                        newNode = node;
            }
            else
            {
                createCommand = new Commands.CreateNewDocument(Project.Path, "txt");
                InvokeCommand(createCommand);
                UpdateNode(null);
                foreach (TreeNode node in treeView.Nodes)
                    if ((node.Tag as NodeTag).Path == Project.Path + "\\" + createCommand.NewFileName)
                        newNode = node;
            }

            newNode.EnsureVisible();
            treeView.SelectedNode = newNode;
            newNode.BeginEdit();
        }

        private void NewFolderMenuItemHandler(object sender, EventArgs e)
        {
            Commands.CreateFolder createCommand = null;
            TreeNode newNode = null;

            if (ContextNode != null)
            {
                var tag = ContextNode.Tag as NodeTag;
                var dirPath = tag.Path;
                if (tag.NodeType == NodeTag.Type.File)
                    dirPath = System.IO.Path.GetDirectoryName(dirPath);
                createCommand = new Commands.CreateFolder(dirPath);
                InvokeCommand(createCommand);
                UpdateNode(ContextNode);
                foreach (TreeNode node in ContextNode.Nodes)
                    if ((node.Tag as NodeTag).Path == dirPath + "\\" + createCommand.NewFileName)
                        newNode = node;
            }
            else
            {
                createCommand = new Commands.CreateFolder(Project.Path);
                InvokeCommand(createCommand);
                UpdateNode(null);
                foreach (TreeNode node in treeView.Nodes)
                    if ((node.Tag as NodeTag).Path == Project.Path + "\\" + createCommand.NewFileName)
                        newNode = node;
            }

            newNode.EnsureVisible();
            treeView.SelectedNode = newNode;
            newNode.BeginEdit();
        }

        private void RefreshWordCountMenuItemHandler(object sender, EventArgs e)
        {
            //InvokeCommand(new Commands.CountWords((ContextNode.Tag as NodeTag).Path));
            var tag = ContextNode.Tag as NodeTag;
            if (tag.NodeType == NodeTag.Type.File)
            {
                // Handle case where document is open?
                tag.WordCount = WordParser.CountWords(System.IO.File.ReadAllText(tag.Path));

            }
            else if (tag.NodeType == NodeTag.Type.Directory)
            {
                // Need to update tags of sub folders.
                var meta = new MetaInformation(tag.Path);
                meta.UpdateFromDisc();
                tag.WordCount = meta.Data.TotalWordCount;
                meta.Save();
            }

            Refresh();
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
                var newPath = directory + "\\" + e.Label;

                InvokeCommand(new Commands.RenameFilesystemItem(tag.Path, newPath));

                tag.Path = newPath;
            }
            else if (tag.NodeType == NodeTag.Type.Directory)
            {
                var directory = System.IO.Path.GetDirectoryName(tag.Path);
                var newPath = directory + "\\" + e.Label;

                InvokeCommand(new Commands.RenameFilesystemItem(tag.Path, newPath));

                tag.Path = newPath;
                UpdateNode(e.Node);
            }
        }

        private bool Confirm(String Text)
        {
            var r = MessageBox.Show(Text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return r == System.Windows.Forms.DialogResult.Yes;
        }

        private void DirectoryListing_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                InvokeCommand(new Commands.CloseFolder(Project));
            }
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (treeView.SelectedNode != null)
                {
                    var tag = treeView.SelectedNode.Tag as NodeTag;
                    if (tag != null && tag.NodeType == NodeTag.Type.File)
                        InvokeCommand(new Commands.OpenPath(tag.Path, Commands.OpenCommand.OpenStyles.CreateView));
                    else if (tag != null)
                        treeView.SelectedNode.Toggle();
                }
            }
        }

        #region Drag and Drop
        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var tag = (e.Item as TreeNode).Tag;
            var nodeTag = tag as NodeTag;
            if (nodeTag.NodeType == NodeTag.Type.Root) return;
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                var sourceNode = e.Data.GetData("System.Windows.Forms.TreeNode") as TreeNode;
                if (sourceNode != null && sourceNode.Tag is NodeTag)
                    e.Effect = DragDropEffects.Move;
            }
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            var sourceNode = e.Data.GetData("System.Windows.Forms.TreeNode") as TreeNode;
            if (sourceNode == null) throw new InvalidProgramException();
            var sourceTag = sourceNode.Tag as NodeTag;
            if (sourceTag == null) throw new InvalidProgramException();

            NodeTag destTag = null;
            var destinationDirectoryPath = "";

            TreeNode destToRebuild = null;
            TreeNode sourceToRebuild = sourceNode.Parent;

            var node = treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));

            if (node != null)
            {
                destTag = node.Tag as NodeTag;
                if (destTag.NodeType == NodeTag.Type.Directory)
                {
                    destinationDirectoryPath = destTag.Path;
                    destToRebuild = node;
                }
                else if (destTag.NodeType == NodeTag.Type.File)
                {
                    destinationDirectoryPath = System.IO.Path.GetDirectoryName(destTag.Path);
                    destToRebuild = node.Parent;
                }
            }
            else
            {
                destinationDirectoryPath = System.IO.Path.GetDirectoryName(Project.Path);
                destToRebuild = null;
            }

            var sourceFileName = System.IO.Path.GetFileName(sourceTag.Path);
            var newPath = destinationDirectoryPath + "\\" + sourceFileName;

            var command = new Commands.RenameFilesystemItem(sourceTag.Path, newPath);
            InvokeCommand(command);
            if (command.Succeeded)
            {
                UpdateNode(destToRebuild);
                UpdateNode(sourceToRebuild);
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            treeView.SelectedNode = treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));
        }
        #endregion

        private void treeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true;

            e.Graphics.FillRectangle(Brushes.White, e.Bounds.Right, e.Bounds.Y,
                (sender as TreeView).Bounds.Right - e.Bounds.Right, e.Bounds.Height);

            if (e.Node.Tag != null)
            {
                e.Graphics.DrawString((e.Node.Tag as NodeTag).WordCount.ToString(), Font, Brushes.Black, (sender as TreeView).Bounds.Right - 64, e.Bounds.Top);
            }
        }

        private void treeView_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            Refresh();
        }
    }
}
