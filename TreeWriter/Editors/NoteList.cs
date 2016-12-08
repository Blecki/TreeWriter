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
    public partial class NoteList : DocumentEditor
    {
        NotesDocument NoteSource;

        private class ScenePosition
        {
            public SceneData Scene;
            public int Place;
        }

        public NoteList(NotesDocument Document) : base(Document)
        {
            this.NoteSource = Document;

            this.InitializeComponent();

            Text = Document.GetTitle();

            var refreshContextMenuItem = new ToolStripMenuItem("Refresh");
            refreshContextMenuItem.Click += refreshToolStripMenuItem_Click;
            this.contextMenuStrip1.Items.Add(refreshContextMenuItem);

            RefreshList();
        }

        public void RefreshList()
        {
            listView.Items.Clear();

            foreach (var scene in NoteSource.ParentDocument.Data.Scenes)
            {
                var index = 0;
                while (index != -1)
                {
                    index = scene.Prose.IndexOf('[', index);
                    if (index != -1)
                    {
                        var end = scene.Prose.IndexOf(']', index);
                        if (end != -1)
                        {
                            var note = scene.Prose.Substring(index + 1, end - index - 1);
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

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList();
        }
        
        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = listView.GetItemAt(e.Location.X, e.Location.Y);
            if (item != null)
            {
                var position = item.Tag as ScenePosition;
                var openCommand = new Commands.OpenPath(Document.Path + "&" + position.Scene.Name + ".$prose",
                    Commands.OpenCommand.OpenStyles.CreateView);
                InvokeCommand(openCommand);
                if (openCommand.Succeeded && openCommand.Document != null)
                {
                    var textEditor = openCommand.Document.OpenEditors.FirstOrDefault(d => d is TextDocumentEditor) as TextDocumentEditor;
                    if (textEditor != null)
                    {
                        textEditor.SetCursorAndScroll(position.Place);
                        textEditor.Focus();
                    }
                }                
            }            
        }
    }
}
