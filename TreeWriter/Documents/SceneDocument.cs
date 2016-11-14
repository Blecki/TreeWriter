using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class SceneDocument : EditableDocument
    {
        public String Name;
        public String Tags;
        public String Summary;
        public String Prose;

        public FolderDocument Owner;

        public override string GetContents()
        {
            return Prose;
        }

        public override string GetEditorTitle()
        {
            return System.IO.Path.GetFileNameWithoutExtension(Name) + (NeedChangesSaved ? "*" : "");
        }

        public override void ApplyChanges(string NewText)
        {
            Prose = NewText;
            base.ApplyChanges(NewText);
        }
    }
}
