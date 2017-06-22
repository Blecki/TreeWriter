using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class NotesDocument : EditableDocument
    {
        public override void Load(Model Model, Main View, string Path)
        {
            this.Path = Path;
            // UI loads and parses.
        }

        public override WeifenLuo.WinFormsUI.Docking.DockState GetPreferredOpeningDockState()
        {
            return WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide;
        }

        protected override string ImplementGetEditorTitle()
        {
            return Path;
        }

        public override string GetContents()
        {
            return "";
        }

        public override int CountWords(Model Model, Main View)
        {
            return 0;
        }

        public override DockablePanel OpenView(Model Model)
        {
            var r = new NoteList(this);
            OpenEditors.Add(r);
            return r;
        }

        public override void ApplyChanges(string NewText)
        {
            MadeChanges();
        }

        public override void Save(bool Backup)
        {
            NeedChangesSaved = false;
            UpdateViewTitles();
        }

        public override void MadeChanges()
        {
            base.MadeChanges();
        }
    }
}
