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

        public override void Load(Model Model, Main View, string Path)
        {
            this.Path = Path;
            Contents = System.IO.File.ReadAllText(Path);
        }

        public override string GetContents()
        {
            return Contents;
        }

        protected override string ImplementGetEditorTitle()
        {
            return System.IO.Path.GetFileName(Path);
        }

        public override int CountWords(Model Model, Main View)
        {
            return WordParser.CountWords(Contents);
        }

        public override void ApplyChanges(string NewText)
        {
 	        Contents = NewText;
            base.ApplyChanges(NewText);
        }

        public override void Save(bool Backup)
        {
            System.IO.File.WriteAllText(Path, Contents);
            NeedChangesSaved = false;
            UpdateViewTitles();

            if (Backup)
                System.IO.File.WriteAllText(GetBackupFilename(), Contents);
        }

        public override DockablePanel OpenView(Model Model)
        {
            var r = new TextDocumentEditor(
                this, 
                OpenEditors.Count != 0 ? (OpenEditors[0] as TextDocumentEditor).GetScintillaDocument() : (ScintillaNET.Document?)null,
                Model.SpellChecker, Model.Thesaurus);
            r.CustomizeContextMenu = (menu) =>
            {
                var item = menu.MenuItems.Add("Cut to new document");
                item.Click += (sender, args) =>
                {
                    var createCommand = new Commands.CreateNewDocument(System.IO.Path.GetDirectoryName(Path), "txt");
                    r.InvokeCommand(createCommand);
                    if (!createCommand.Succeeded)
                    {
                        System.Windows.Forms.MessageBox.Show("Failed to create new file.");
                        return;
                    }

                    var prose = r.textEditor.SelectedText;
                    System.IO.File.WriteAllText(createCommand.FullPath, prose);
                    r.textEditor.ReplaceSelection("");

                    MadeChanges();
                };
            };
            OpenEditors.Add(r);
            return r;
        }

    }
}
