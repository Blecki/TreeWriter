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
    public partial class NoteList : DockablePanel
    {
        ManuscriptDocument Document;

        private class ScenePosition
        {
            public SceneData Scene;
            public int Place;
        }

        public NoteList(ManuscriptDocument Document)
        {
            this.Document = Document;

            this.InitializeComponent();

            Text = Document.GetEditorTitle();
        }

        public void RefreshList()
        {
            listView.Items.Clear();

            foreach (var scene in Document.Data.Scenes)
            {
                var index = 0;
                while (index != -1)
                {
                    index = scene.Summary.IndexOf('[', index);
                    if (index != -1)
                    {
                        var end = scene.Summary.IndexOf(']', index);
                        if (end != -1)
                        {
                            var note = scene.Summary.Substring(index + 1, end - index - 1);
                            var item = new ListViewItem(new string[] { note, scene.Name });
                            item.Tag = new ScenePosition { Scene = scene, Place = index };
                            listView.Items.Add(item);
                        }
                        index = end;
                    }
                }
            }

            listView.Columns[0].Width = -1;
            listView.Columns[1].Width = -1;
        }

        private void NoteList_FormClosing(object sender, FormClosingEventArgs e)
        {
            Document.OpenEditors.Remove(this);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList();
        }
        
        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = listView.GetItemAt(e.Location.X, e.Location.Y);
            if (item != null)
            {
                ManuscriptDocumentEditor editor = Document.OpenEditors.FirstOrDefault(d => d is ManuscriptDocumentEditor)
                    as ManuscriptDocumentEditor;
                if (editor == null)
                {
                    InvokeCommand(new Commands.DuplicateView(Document));
                    editor = Document.OpenEditors.FirstOrDefault(d => d is ManuscriptDocumentEditor)
                        as ManuscriptDocumentEditor;
                }
                editor.BringSceneToFront((item.Tag as ScenePosition).Scene, (item.Tag as ScenePosition).Place);  
            }            
        }
    }
}
