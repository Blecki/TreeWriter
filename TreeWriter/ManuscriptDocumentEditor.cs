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
    public partial class ManuscriptDocumentEditor : DockablePanel
    {
        private ManuscriptDocument Document;
        private ListViewItem ContextNode;
        private SceneData SelectedScene = null;
        private bool SuppressTextChange = false;
        private bool SuppressTagTextChange = false;

        public ManuscriptDocumentEditor(ManuscriptDocument Document)
        {
            this.Document = Document;

            InitializeComponent();

            UpdateList();
            listView_SelectedIndexChanged(null, null);

            Text = Document.GetEditorTitle();
        }

        private void UpdateList()
        {
            int selectedIndex = -1;
            if (listView.SelectedItems.Count > 0)
                selectedIndex = Document.Data.Scenes.IndexOf(listView.SelectedItems[0].Tag as SceneData);

            ListViewItem itemToSelect = null;

            listView.Items.Clear();
            for (int i = 0; i < Document.Data.Scenes.Count; ++i)
            {
                var scene = Document.Data.Scenes[i];
                if (scene.Tags.Contains(filterBox.Text))
                {
                    var item = new ListViewItem(new String[] { scene.Name, scene.Tags })
                    {
                        Tag = scene
                    };
                    if (i == selectedIndex) itemToSelect = item;
                    listView.Items.Add(item);
                }
            }

            if (itemToSelect != null)
                listView.SelectedIndices.Add(itemToSelect.Index);

            listView.Invalidate();
        }

        public override void ReloadDocument()
        {
            UpdateList();
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseStyle == EditableDocument.CloseStyle.Natural && e.CloseReason == CloseReason.UserClosing)
            {
                var closeCommand = new Commands.CloseEditor(Document, this, e.CloseReason == CloseReason.MdiFormClosing);
                InvokeCommand(closeCommand);
                e.Cancel = closeCommand.Cancel;
            }
        }

        private void listView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextNode = listView.GetItemAt(e.X, e.Y);
                deleteSceneToolStripMenuItem.Enabled = ContextNode != null;
                contextMenu.Show(this, e.Location);
            }
        }

        private bool Confirm(String Text)
        {
            var r = MessageBox.Show(Text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return r == System.Windows.Forms.DialogResult.Yes;
        }

        private void DocumentEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                InvokeCommand(new Commands.SaveDocument(Document));
            }
        }

        private void newSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var contextIndex = Document.Data.Scenes.Count();
            if (ContextNode != null) contextIndex = ContextNode.Index + 1;
            var command = new Commands.CreateScene(
                ContextNode == null ? null : (ContextNode.Tag as SceneData),
                Document);
            InvokeCommand(command);
            if (command.Succeeded)
            {
                UpdateList();
                listView.Items[contextIndex].BeginEdit();
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                SelectedScene = null;
                SuppressTextChange = true;
                SuppressTagTextChange = true;
                textEditor.Text = "";
                textEditor.Enabled = false;
                tagBox.Enabled = false;
                tagBox.Text = "";
                openSceneLabel.Text = "";
            }
            else
            {
                SelectedScene = listView.SelectedItems[0].Tag as SceneData;
                SuppressTextChange = true;
                SuppressTagTextChange = true;
                textEditor.Text = SelectedScene.Summary;
                textEditor.Enabled = true;
                tagBox.Enabled = true;
                tagBox.Text = SelectedScene.Tags;
                openSceneLabel.Text = SelectedScene.Name;
            }
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            if (SuppressTextChange)
            {
                SuppressTextChange = false;
                return;
            }

            if (SelectedScene != null)
            {
                SelectedScene.Summary = textEditor.Text;
                Document.MadeChanges();
            }
        }

        private void tagBox_TextChanged(object sender, EventArgs e)
        {
            if (SuppressTagTextChange)
            {
                SuppressTagTextChange = false;
                return;
            }

            if (SelectedScene != null)
            {
                SelectedScene.Tags = tagBox.Text;
                Document.MadeChanges();
                listView.SelectedItems[0].SubItems[1].Text = tagBox.Text;
            }
        }

        private void listView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            var item = listView.Items[e.Item].Tag as SceneData;
            InvokeCommand(new Commands.RenameScene(Document, item, e.Label));
        }

        private void listView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            listView.DoDragDrop(listView.SelectedItems, DragDropEffects.Move);
        }

        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            int len = e.Data.GetFormats().Length - 1;
            int i;
            for (i = 0; i <= len; i++)
            {
                if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection"))
                {
                    //The data from the drag source is moved to the target.	
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void listView_DragDrop(object sender, DragEventArgs e)
        {
            if (listView.SelectedItems.Count == 0) return;
            var p = PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = listView.GetItemAt(p.X, p.Y);
            
            if (dragToItem == null)
            {
                Document.Data.Scenes.Remove(listView.SelectedItems[0].Tag as SceneData);
                Document.Data.Scenes.Add(listView.SelectedItems[0].Tag as SceneData);
                UpdateList();
            }
            else
            {
                var insertAfter = dragToItem.Tag as SceneData;
                var insertIndex = Document.Data.Scenes.IndexOf(insertAfter);
                var sourceIndex = Document.Data.Scenes.IndexOf(listView.SelectedItems[0].Tag as SceneData);
                Document.Data.Scenes.Remove(listView.SelectedItems[0].Tag as SceneData);
                Document.Data.Scenes.Insert(sourceIndex > insertIndex ? (insertIndex) : (insertIndex - 1),
                    listView.SelectedItems[0].Tag as SceneData);
                UpdateList();
            }
        }

        private void deleteSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ContextNode != null)
            {
                var scene = ContextNode.Tag as SceneData;
                InvokeCommand(new Commands.DeleteScene(Document, scene));
            }
        }

        private void clearFilterButton_Click(object sender, EventArgs e)
        {
            filterBox.Text = "";
            UpdateList();
        }

        private void refreshFilterButton_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void filterBox_TextChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void duplicateViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvokeCommand(new Commands.DuplicateView(Document));
        }        
    }
}
