using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class RenameFolder :ICommand
    {
        private String OriginalName;
        private String NewName;

        public RenameFolder(String OriginalName, String NewName)
        {
            this.OriginalName = OriginalName;
            this.NewName = NewName;
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
                return;
            }
            
            // If the document is a directory, look for any open documents in that folder, and update their paths.
            foreach (var document in Model.OpenDocuments.Where(d => d.FileName.StartsWith(OriginalName)))
            {
                document.FileName = NewName + (document.FileName.Substring(OriginalName.Length));
                foreach (var editor in document.OpenEditors)
                    editor.UpdateTitle();
            }

            // Guess the directory listing will update itself?
        }
    }
}
