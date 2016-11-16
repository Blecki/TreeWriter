using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class SceneDocument : EditableDocument
    {
        public SceneData Data;
        public ManuscriptDocument Owner;
     
        public override string GetContents()
        {
            return Data.Prose;
        }

        public override string GetEditorTitle()
        {
            return Data.Name + (NeedChangesSaved ? "*" : "");
        }

        public override void ApplyChanges(string NewText)
        {
            Data.Prose = NewText;
            Owner.NeedChangesSaved = true;
            base.ApplyChanges(NewText);
        }

        public override void SaveDocument()
        {
            Owner.SaveDocument();
        }

        public override EditableDocument GetRootDocument()
        {
            return Owner;
        }

        public override ControllerPanel OpenView(Model Model)
        {
            var r = new DocumentEditor(this, null, Model.SpellChecker, Model.Thesaurus);
            OpenEditors.Add(r);
            return r;
        }
    }
}
