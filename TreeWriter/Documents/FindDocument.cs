using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class FindDocument : EditableDocument
    {
        public EditableDocument ParentDocument;

        private String[] ParsePathParts(String Path)
        {
            var index = Path.LastIndexOf('&');
            if (index == -1) return new String[] { Path, null };
            return new String[] { Path.Substring(0, index), Path.Substring(index + 1, Path.Length - index - 1) };
        }

        public override void Load(Model Model, Main View, string Path)
        {
            this.Path = Path;
            var split = ParsePathParts(Path);
            var openParent = new Commands.OpenPath(split[0], Commands.OpenCommand.OpenStyles.CreateView);
            openParent.Execute(Model, View);
            if (!openParent.Succeeded) throw new InvalidOperationException();
            if (openParent.Document == null) throw new InvalidOperationException();
            ParentDocument = openParent.Document;
        }

        public override WeifenLuo.WinFormsUI.Docking.DockState GetPreferredOpeningDockState()
        {
            return WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide;
        }

        protected override string ImplementGetEditorTitle()
        {
            return System.IO.Path.GetFileNameWithoutExtension(ParentDocument.Path) + " - $ Find";
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
            var r = new Find(this);
            OpenEditors.Add(r);
            return r;
        }

        public override void ApplyChanges(string NewText)
        {
        }

        public override void Save(bool Backup)
        {
        }

        public override void MadeChanges()
        {
        }
    }
}
