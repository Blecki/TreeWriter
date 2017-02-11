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
        private SceneData ContextScene;
        private SceneData MouseDownScene;
        private Point MouseDownPoint;
        private ManuscriptDocument ManuDoc;

        public ManuscriptDocumentEditor(ManuscriptDocument Document) : base(Document)
        {
            ManuDoc = Document;

            InitializeComponent();

            UpdateList();
            Text = Document.GetTitle();

            var notesContextMenuItem = new ToolStripMenuItem("Notes");
            notesContextMenuItem.Click += notesContextMenuItem_Click;
            this.contextMenuStrip1.Items.Add(notesContextMenuItem);

            var extractMenuItem = new ToolStripMenuItem("Extract Manuscript");
            extractMenuItem.Click += extractMenuItem_Click;
            this.contextMenuStrip1.Items.Add(extractMenuItem);
        }

        void extractMenuItem_Click(object sender, EventArgs e)
        {
            InvokeCommand(new Commands.Extract.OpenManuscriptExtractor(Document as ManuscriptDocument));
        }

        void notesContextMenuItem_Click(object sender, EventArgs e)
        {
            InvokeCommand(new Commands.OpenPath(Document.Path + "&notes.$notes", Commands.OpenCommand.OpenStyles.CreateView));
        }

        private void UpdateList()
        {
            int selectedIndex = -1;
            if (dataGridView.SelectedCells.Count > 0)
                selectedIndex = ManuDoc.Data.Scenes.IndexOf(dataGridView.SelectedCells[0].OwningRow.Tag as SceneData);
            var topRow = dataGridView.FirstDisplayedScrollingRowIndex;

            DataGridViewRow selectedRow = null;
            dataGridView.DataSource = null;
            dataGridView.Rows.Clear();
            for (int i = 0; i < ManuDoc.Data.Scenes.Count; ++i)
            {
                var scene = ManuDoc.Data.Scenes[i];
                if (scene.Tags.Contains(filterBox.Text))
                {
                   
                    var item = new DataGridViewRow();
                    item.CreateCells(dataGridView, scene.Name, scene.Tags, WordParser.CountWords(scene.Prose).ToString());
                    item.Tag = scene;
                    item.DefaultCellStyle.BackColor = scene.Color;
                    if (i == selectedIndex) selectedRow = item;
                    dataGridView.Rows.Add(item);
                }
            }

            if (selectedRow != null)
                selectedRow.Selected = true;

            if (topRow >= dataGridView.Rows.Count) topRow = dataGridView.Rows.Count - 1;
            if (topRow < 0) topRow = 0;
            dataGridView.FirstDisplayedScrollingRowIndex = topRow;
        }

        private void RebuildListItem(int Index)
        {
            var row = dataGridView.Rows[Index];
            var scene = row.Tag as SceneData;
            row.Cells[0].Value = scene.Name;
            row.Cells[1].Value = scene.Tags;
            row.Cells[2].Value = WordParser.CountWords(scene.Prose).ToString();
            row.DefaultCellStyle.BackColor = scene.Color;
        }

        public void RebuildLineItem(SceneData Scene)
        {
            var listIndex = FindListIndexOfScene(Scene);
            if (listIndex != -1)
                RebuildListItem(listIndex);
        }

        private SceneData GetSelectedScene()
        {
            if (dataGridView.SelectedCells.Count == 0) return null;
            return dataGridView.SelectedCells[0].OwningRow.Tag as SceneData;
        }

        private SceneData GetSceneForRow(int RowIndex)
        {
            if (RowIndex < 0 || RowIndex >= dataGridView.Rows.Count) return null;
            return dataGridView.Rows[RowIndex].Tag as SceneData;
        }

        public override void ReloadDocument()
        {
            UpdateList();
        }

        private bool Confirm(String Text)
        {
            var r = MessageBox.Show(Text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return r == System.Windows.Forms.DialogResult.Yes;
        }

        private void newSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Bug - New scenes are drawn wrong until the first time they are modified. Possibly because props are null?
            // TODO: Make sure scene is selected and visible, and enter edit mode.
            var command = new Commands.CreateScene(ContextScene, ManuDoc);
            InvokeCommand(command);
            if (command.Succeeded)
                UpdateList();


        }        

        private void deleteSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ContextScene != null)
            {
                if (Confirm("Are you sure you want to delete this scene?"))
                    InvokeCommand(new Commands.DeleteScene(ManuDoc, ContextScene));
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

        public int FindListIndexOfScene(SceneData Scene)
        {
            for (var itemIndex = 0; itemIndex < dataGridView.Rows.Count; ++itemIndex)
                if (dataGridView.Rows[itemIndex].Tag == Scene) return itemIndex;
            return -1;
        }

        public void BringSceneToFront(SceneData Scene, int Location)
        {
            // TODO: Find or open scene editor and navigate to location.
        }

        private void OpenSceneEditor(SceneData Scene)
        {
            var openCommand = new Commands.OpenPath(Document.Path + "&" + Scene.Name + ".$prose", Commands.OpenCommand.OpenStyles.CreateView);
            InvokeCommand(openCommand);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedScene = GetSelectedScene();
            if (selectedScene == null) return;
            var openCommand = new Commands.OpenPath(Document.Path + "&" + selectedScene.Name + ".$settings",
                Commands.OpenCommand.OpenStyles.CreateView);
            InvokeCommand(openCommand);
            if (openCommand.Succeeded && openCommand.Document != null)
            {
                (openCommand.Document.OpenEditors[0] as DocumentSettingsEditor).OnObjectPropertyChange += () =>
                    {
                        RebuildLineItem(selectedScene);
                    };
            }
        }

        private void dataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var row = dataGridView.Rows[e.RowIndex];
            var scene = row.Tag as SceneData;
            var colorPicker = new ColorDialog();
            var dResult = colorPicker.ShowDialog();
            if (dResult == System.Windows.Forms.DialogResult.OK)
            {
                scene.Color = colorPicker.Color;
                Document.MadeChanges();
                RebuildListItem(e.RowIndex);
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridView.Rows[e.RowIndex];
            var scene = row.Tag as SceneData;
            var value = row.Cells[e.ColumnIndex].Value.ToString();

            if (e.ColumnIndex == 0) // Edit Name
            {
                if (String.IsNullOrEmpty(value))
                {
                    row.Cells[e.ColumnIndex].Value = scene.Name;
                }
                else
                {
                    InvokeCommand(new Commands.RenameScene(ManuDoc, scene, value));
                }
            }
            else if (e.ColumnIndex == 1) // Edit tags
            {
                scene.Tags = value;
                Document.MadeChanges();
            }
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.EditingControl != null) return;
            var scene = GetSceneForRow(e.RowIndex);
            if (scene != null) OpenSceneEditor(scene);
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                var scene = GetSelectedScene();
                if (scene != null) OpenSceneEditor(scene);
            }
            else
                base.DocumentEditor_KeyDown(sender, e);
        }

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            var clickRow = dataGridView.HitTest(e.X, e.Y).RowIndex;
            dataGridView.ClearSelection();
            if (clickRow >= 0 && clickRow < dataGridView.Rows.Count)
            {
                MouseDownScene = GetSceneForRow(clickRow);
                MouseDownPoint = Control.MousePosition;
                dataGridView.Rows[clickRow].Selected = true;
            }
        }

        private void dataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseDownScene != null)
            {
                var mousePoint = Control.MousePosition;
                var xDelta = System.Math.Abs(mousePoint.X - MouseDownPoint.X);
                var yDelta = System.Math.Abs(mousePoint.Y - MouseDownPoint.Y);
                if (xDelta > 10 || yDelta > 10)
                    dataGridView.DoDragDrop(MouseDownScene, DragDropEffects.Move);
            }
        }

        private void dataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            MouseDownScene = null;

            if (e.Button == MouseButtons.Right)
            {
                var hit = dataGridView.HitTest(e.X, e.Y);

                if (hit.RowIndex >= 0 && hit.RowIndex < dataGridView.Rows.Count && hit.ColumnIndex >= 0 && hit.ColumnIndex < dataGridView.Columns.Count)
                {
                    dataGridView.ClearSelection();
                    dataGridView.Rows[hit.RowIndex].Cells[hit.ColumnIndex].Selected = true;
                }

                ContextScene = GetSceneForRow(hit.RowIndex);
                deleteSceneToolStripMenuItem.Enabled = ContextScene != null;
                propertiesToolStripMenuItem.Enabled = ContextScene != null;
                contextMenu.Show(dataGridView, dataGridView.PointToClient(Control.MousePosition));
            }

            
        }

        private void dataGridView_DragEnter(object sender, DragEventArgs e)
        {
            if (MouseDownScene != null)
                e.Effect = DragDropEffects.Move;
        }

        private void dataGridView_DragDrop(object sender, DragEventArgs e)
        {
            var clientPoint = dataGridView.PointToClient(new Point(e.X, e.Y));
            var dropRow = dataGridView.HitTest(clientPoint.X, clientPoint.Y);

            if (e.Effect == DragDropEffects.Move)
            {
                if (dropRow.RowIndex < 0)
                {
                    ManuDoc.Data.Scenes.Remove(MouseDownScene);
                    ManuDoc.Data.Scenes.Insert(0, MouseDownScene);
                }
                else if (dropRow.RowIndex >= dataGridView.Rows.Count)
                {
                    ManuDoc.Data.Scenes.Remove(MouseDownScene);
                    ManuDoc.Data.Scenes.Add(MouseDownScene);
                }
                else
                {
                    var insertAfter = GetSceneForRow(dropRow.RowIndex);
                    ManuDoc.Data.Scenes.Remove(MouseDownScene);
                    ManuDoc.Data.Scenes.Insert(ManuDoc.Data.Scenes.IndexOf(insertAfter) + 1, MouseDownScene);
                }

                ManuDoc.MadeChanges();
                UpdateList();
            }

            MouseDownScene = null;
        }
    }
}
