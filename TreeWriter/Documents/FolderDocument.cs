using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class FolderDocument : EditableDocument
    {
        public override void Load(Model Model, Main View, string Path)
        {
            this.Path = Path;
        }

        public override DockablePanel OpenView(Model Model)
        {
            var r = new DirectoryListing(this);
            OpenEditors.Add(r);
            return r;
        }

        protected override string ImplementGetEditorTitle()
        {
            return Path;
        }

        public override WeifenLuo.WinFormsUI.Docking.DockState GetPreferredOpeningDockState()
        {
            return WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
        }

        public override int CountWords(Model Model, Main View)
        {
            return TotalDirectory(Path, Model, View);
        }

        private int TotalDirectory(String Path, Model Model, Main View)
        {
            // TODO: Don't count scrap.txt

            var total = 0;
            foreach (var directory in System.IO.Directory.EnumerateDirectories(Path))
                total += TotalDirectory(directory, Model, View);
            foreach (var file in System.IO.Directory.EnumerateFiles(Path))
                total += TotalDocument(file, Model, View);
            return total;
        }

        private int TotalDocument(String Path, Model Model, Main View)
        {
            var openCommand = new Commands.OpenPath(Path, Commands.OpenCommand.OpenStyles.Transient);
            openCommand.Execute(Model, View);
            if (openCommand.Document != null)
                return openCommand.Document.CountWords(Model, View);
            else
                return 0;
        }
    }
}
