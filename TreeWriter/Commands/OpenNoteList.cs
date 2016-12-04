using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenNoteList :ICommand
    {
        private ManuscriptDocument Document;
        public bool Succeeded { get; private set; }

        public OpenNoteList(ManuscriptDocument Document)
        {
            this.Document = Document;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            NoteList existingNoteList = Document.OpenEditors.FirstOrDefault(v => v is NoteList) as NoteList;
            if (existingNoteList == null)
            {
                existingNoteList = new NoteList(Document);
                View.OpenControllerPanel(existingNoteList, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                Document.OpenEditors.Add(existingNoteList);
            }
            existingNoteList.Focus();
            existingNoteList.RefreshList();
            Succeeded = true;
        }
    }
}
