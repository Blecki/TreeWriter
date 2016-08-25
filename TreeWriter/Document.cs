using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class Document
    {
        public String FileName;
        public String Contents;
        public bool NeedChangesSaved = false;
        public List<DocumentEditor> OpenEditors = new List<DocumentEditor>();
        public Project Owner;
        
        // Todo: Watch file and update it if it's changed by another program.

        public void ApplyChanges(String NewText)
        {
            Contents = NewText;
            NeedChangesSaved = true;
            // If editors are properly linked through scintilla documents, they should already share changes.
            foreach (var editor in OpenEditors) editor.UpdateTitle();
        }
    }
}
