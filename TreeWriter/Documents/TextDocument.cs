using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class TextDocument : EditableDocument
    {
        public String Contents;

        public override string GetContents()
        {
            return Contents;
        }

        public override string GetEditorTitle()
        {
            return System.IO.Path.GetFileNameWithoutExtension(Path) + (NeedChangesSaved ? "*" : "");
        }

        public override void ApplyChanges(string NewText)
        {
 	        Contents = NewText;
            base.ApplyChanges(NewText);
        }

        public override void SaveDocument()
        {
            System.IO.File.WriteAllText(Path, Contents);
            NeedChangesSaved = false;
        }

        public override Model.SerializableDocument GetSerializableDocument()
        {
            return new Model.SerializableDocument
            {
                Path = Path,
                Type = "TEXT"
            };
        }

        public override ControllerPanel OpenView(Model Model)
        {
            var r = new DocumentEditor(this, null, Model.SpellChecker, Model.Thesaurus);
            OpenEditors.Add(r);
            return r;
        }

    }
}
