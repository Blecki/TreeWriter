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

        private class NotePosition
        {
            public String Path;
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

        private void ScanDirectory(String Path)
        {
            foreach (var subDir in Model.EnumerateDirectories(Path))
                ScanDirectory(subDir);

            foreach (var file in Model.EnumerateFiles(Path))
                ScanFile(file);
        }

        private void ScanFile(String Path)
        {
            var prose = System.IO.File.ReadAllText(Path);
            var index = 0;
            while (index != -1)
            {
                index = prose.IndexOf('[', index);
                if (index != -1)
                {
                    var end = prose.IndexOf(']', index);
                    if (end != -1)
                    {
                        var note = prose.Substring(index + 1, end - index - 1);
                        var item = new ListViewItem(new string[] { note, Path });
                        item.Tag = new NotePosition { Path = Path, Place = index };
                        listView.Items.Add(item);
                    }
                    index = end;
                }
            }
        }

        public void RefreshList()
        {
            listView.Items.Clear();

            var realPath = NoteSource.Path.Substring(0, NoteSource.Path.Length - ".$notes".Length);

            if (System.IO.Directory.Exists(realPath))
                ScanDirectory(realPath);
            else
                ScanFile(realPath);
            
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
                var position = item.Tag as NotePosition;
                var openCommand = new Commands.OpenPath(position.Path,
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
