using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class EditableDocument
    {
        public String Path;
        public bool NeedChangesSaved = false;
        public List<ControllerPanel> OpenEditors = new List<ControllerPanel>();
        
        public virtual void ApplyChanges(String NewText)
        {
            NeedChangesSaved = true;
            // If editors are properly linked through scintilla documents, they should already share changes.
            foreach (var editor in OpenEditors) editor.Text = this.GetEditorTitle();
        }

        public virtual String GetContents() { throw new NotImplementedException(); }
        
        public virtual String GetEditorTitle()
        {
            throw new NotImplementedException();
            //System.IO.Path.GetFileNameWithoutExtension(Document.FileName) + (Document.NeedChangesSaved ? "*" : "");
        }

        public virtual EditableDocument GetRootDocument() { return this; }

        public virtual Model.SerializableDocument GetSerializableDocument() { return null; }

        public virtual void SaveDocument() { throw new NotImplementedException(); }

        public virtual void CloseAllViews() 
        {
            var editors = new List<ControllerPanel>(OpenEditors);
            foreach (var editor in editors)
                editor.Close();
        }

        public virtual ControllerPanel OpenView(Model Model)
        {
            throw new NotImplementedException();
        }
    }
}
