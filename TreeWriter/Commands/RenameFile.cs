using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class RenameFile :ICommand
    {
        private String OriginalFileName;
        private String NewFileName;
        
        public RenameFile(String OriginalFileName, String NewFileName)
        {
            this.OriginalFileName = OriginalFileName;
            this.NewFileName = NewFileName;
        }

        public void Execute(ProjectModel Model, Main View)
        {
            // Try and rename the actual on disc file.
            try
            {
                System.IO.File.Move(OriginalFileName, NewFileName);
            }
            catch (Exception e)
            {
                // If it fails, give up.
                return;
            }

            // If the document is open, update the document - and all the open editors.
            foreach (var document in Model.OpenDocuments.Where(d => d.FileName == OriginalFileName))
            {
                document.FileName = NewFileName;
                foreach (var editor in document.OpenEditors)
                    editor.UpdateTitle();
            }

            // If the document is a directory, look for any open documents in that folder, and update their paths.
            foreach (var document in Model.OpenDocuments.Where(d => d.FileName.StartsWith(OriginalFileName)))
            {
                document.FileName = NewFileName + (document.FileName.Substring(OriginalFileName.Length));
                foreach (var editor in document.OpenEditors)
                    editor.UpdateTitle();
            }

            // Guess the directory listing will update itself?
        }
    }
}
