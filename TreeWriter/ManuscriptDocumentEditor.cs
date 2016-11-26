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
    public partial class ManuscriptDocumentEditor : DocumentEditor
    {
        private ListViewItem ContextNode;
        private ManuscriptDocument ManuDoc;
        private SceneData SelectedScene = null;
        private bool SuppressTextChange = false;
        private bool SuppressTagTextChange = false;

        public ManuscriptDocumentEditor(
            ManuscriptDocument Document,
            NHunspell.Hunspell SpellChecker,
            NHunspell.MyThes Thesaurus) : base(Document)
        {
            ManuDoc = Document;

            InitializeComponent();

            textEditor.Create(SpellChecker, Thesaurus, (a) => InvokeCommand(a));

            UpdateList();
            listView_SelectedIndexChanged(null, null);
            Text = Document.GetEditorTitle();

            restoreLeft.Visible = false;
            restoreRight.Visible = false;
            collapseLeft.Visible = true;
            collapseRight.Visible = true;
        }

        private void UpdateList()
        {
            int selectedIndex = -1;
            if (listView.SelectedItems.Count > 0)
                selectedIndex = ManuDoc.Data.Scenes.IndexOf(listView.SelectedItems[0].Tag as SceneData);

            ListViewItem itemToSelect = null;

            listView.Items.Clear();
            for (int i = 0; i < ManuDoc.Data.Scenes.Count; ++i)
            {
                var scene = ManuDoc.Data.Scenes[i];
                if (scene.Tags.Contains(filterBox.Text))
                {
                    var item = new ListViewItem(new String[] 
                    { 
                        scene.Name,
                        "",
                        scene.Tags, 
                        WordParser.CountWords(scene.Summary).ToString() 
                    })
                    {
                        Tag = scene,
                        BackColor = Color.FromArgb(scene.Color)
                    };
                    if (i == selectedIndex) itemToSelect = item;
                    listView.Items.Add(item);
                }
            }

            if (itemToSelect != null)
            {
                listView.SelectedIndices.Add(itemToSelect.Index);
                itemToSelect.EnsureVisible();
            }

            listView.Invalidate();
        }

        public override void ReloadDocument()
        {
            UpdateList();
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
           if (e.KeyCode == Keys.D && e.Control)
            {
                //Send to scrap file.

                var text = textEditor.SelectedText;
                if (!String.IsNullOrEmpty(text))
                {
                    InvokeCommand(new Commands.SendToScrap(text, Document.Path));
                    textEditor.ReplaceSelection("");
                }
            }
        }

        private void newSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var contextIndex = ManuDoc.Data.Scenes.Count();
            if (ContextNode != null) contextIndex = ContextNode.Index + 1;
            var command = new Commands.CreateScene(
                ContextNode == null ? null : (ContextNode.Tag as SceneData),
                ManuDoc);
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
                if (!String.IsNullOrEmpty(textEditor.Text)) SuppressTextChange = true;
                if (!String.IsNullOrEmpty(tagBox.Text)) SuppressTagTextChange = true;
                textEditor.Text = "";
                textEditor.Enabled = false;
                tagBox.Enabled = false;
                tagBox.Text = "";
                openSceneLabel.Text = "";
            }
            else
            {
                SelectedScene = listView.SelectedItems[0].Tag as SceneData;
                if (!String.IsNullOrEmpty(SelectedScene.Summary)) SuppressTextChange = true;
                if (!String.IsNullOrEmpty(SelectedScene.Tags)) SuppressTagTextChange = true;
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
                var count = WordParser.CountWords(textEditor.Text);
                listView.SelectedItems[0].SubItems[3].Text = count.ToString();
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
                listView.SelectedItems[0].SubItems[2].Text = tagBox.Text;
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
            InvokeCommand(new Commands.RenameScene(ManuDoc, item, e.Label));
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
                ManuDoc.Data.Scenes.Remove(listView.SelectedItems[0].Tag as SceneData);
                ManuDoc.Data.Scenes.Add(listView.SelectedItems[0].Tag as SceneData);
                UpdateList();
            }
            else
            {
                var insertAfter = dragToItem.Tag as SceneData;
                var insertIndex = ManuDoc.Data.Scenes.IndexOf(insertAfter);
                var sourceIndex = ManuDoc.Data.Scenes.IndexOf(listView.SelectedItems[0].Tag as SceneData);
                ManuDoc.Data.Scenes.Remove(listView.SelectedItems[0].Tag as SceneData);
                ManuDoc.Data.Scenes.Insert(sourceIndex > insertIndex ? (insertIndex) : (insertIndex - 1),
                    listView.SelectedItems[0].Tag as SceneData);
                UpdateList();
            }
        }

        private void deleteSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ContextNode != null)
            {
                var scene = ContextNode.Tag as SceneData;
                if (Confirm("Are you sure you want to delete this scene?")) 
                    InvokeCommand(new Commands.DeleteScene(ManuDoc, scene));
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

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Location.X > listView.Columns[0].Width && e.Location.X < listView.Columns[0].Width + listView.Columns[1].Width)
            {
                var item = listView.GetItemAt(e.Location.X, e.Location.Y);
                if (item != null)
                {
                    var colorPicker = new ColorDialog();
                    var dResult = colorPicker.ShowDialog();
                    if (dResult == System.Windows.Forms.DialogResult.OK)
                    {
                        (item.Tag as SceneData).Color = colorPicker.Color.ToArgb();
                        Document.MadeChanges();
                        item.BackColor = colorPicker.Color;
                    }
                }
            }
        }

        private void collapseRight_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = true;
            collapseLeft.Visible = false;
            collapseRight.Visible = false;
            restoreRight.Visible = true;
            restoreLeft.Visible = false;
        }

        private void restoreLeft_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = false;
            splitContainer1.Panel2Collapsed = false;
            restoreLeft.Visible = false;
            restoreRight.Visible = false;
            collapseLeft.Visible = true;
            collapseRight.Visible = true;
        }

        private void collapseLeft_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = true;
            collapseLeft.Visible = false;
            collapseRight.Visible = false;
            restoreLeft.Visible = true;
            restoreRight.Visible = false;
        }        
    }
}
