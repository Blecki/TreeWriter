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
using ScintillaNET;

namespace TreeWriterWF
{
    public partial class Find : DocumentEditor
    {
        private class TextPosition
        {
            public String Path;
            public int Place;
        }

        public Find(FindDocument Document) : base(Document)
        {
            this.InitializeComponent();

            Text = Document.GetTitle();
            
            RefreshList();
        }

        private IEnumerable<int> SearchText(String Text, String For)
        {
            var index = -1;
            do
            {
                index = Text.IndexOf(For, index + 1);
                if (index != -1)
                    yield return index;
            } 
            while (index != -1);
        }

        private void AddFoundText(String Path, int Position)
        {
            var item = new DataGridViewRow();
            item.CreateCells(dataGridView1, String.Format("{0} : {1}", Path, Position));
            item.Tag = new TextPosition { Path = Path, Place = Position };
            dataGridView1.Rows.Add(item);
        }

        public void RefreshList()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();

            if (String.IsNullOrEmpty(filterBox.Text)) return;

            var findDoc = Document as FindDocument;
            if (findDoc == null) return;
            var manuDoc = findDoc.ParentDocument as ManuscriptDocument;
            if (manuDoc != null)
            {
                foreach (var scene in manuDoc.Data.Scenes)
                    foreach (var item in SearchText(scene.Prose, filterBox.Text))
                        AddFoundText(manuDoc.Path + "&" + scene.Name + ".$prose", item);
            }
            else
            {
                foreach (var item in SearchText(findDoc.ParentDocument.GetContents(), filterBox.Text))
                    AddFoundText(findDoc.ParentDocument.Path, item);
            }
        }

        private void clearFilterButton_Click(object sender, EventArgs e)
        {
            filterBox.Text = "";
            RefreshList();
        }

        private void refreshFilterButton_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;
            var position = dataGridView1.Rows[e.RowIndex].Tag as TextPosition;
            var openCommand = new Commands.OpenPath(position.Path, Commands.OpenCommand.OpenStyles.CreateView);
            InvokeCommand(openCommand);
            if (openCommand.Succeeded && openCommand.Document != null)
            {
                var textEditor = openCommand.Document.OpenEditors.FirstOrDefault(d => d is TextDocumentEditor) as TextDocumentEditor;
                if (textEditor != null)
                    textEditor.SetCursorAndScroll(position.Place);
            }
        }
    }
}
