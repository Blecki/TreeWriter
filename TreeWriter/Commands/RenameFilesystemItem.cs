using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class RenameFilesystemItem :ICommand
    {
        private String OriginalName;
        private String NewName;
        public bool Succeeded { get; private set; }

        public RenameFilesystemItem(String OriginalName, String NewName)
        {
            this.OriginalName = OriginalName;
            this.NewName = NewName;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            // Try and rename the actual on disc file.
            try
            {
                System.IO.Directory.Move(OriginalName, NewName);
            }
            catch (Exception e)
            {
                // If it fails, give up.
                Succeeded = false;
                return;
            }
            
            // If the document is a directory, look for any open documents in that folder, and update their paths.
            foreach (var document in Model.FindChildDocuments(OriginalName))
            {
                document.Path = NewName + (document.Path.Substring(OriginalName.Length));
                foreach (var editor in document.OpenEditors)
                    editor.Text = document.GetEditorTitle();
            }

            Succeeded = true;
            // Guess the directory listing will update itself?
        }
    }
}
