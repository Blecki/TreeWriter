using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class RenameDocument :ICommand
    {
        private String OriginalFileName;
        private String NewFileName;
        
        public RenameDocument(String OriginalFileName, String NewFileName)
        {
            this.OriginalFileName = OriginalFileName;
            this.NewFileName = NewFileName;
        }

        public void Execute(Model Model, Main View)
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
            
            // Guess the directory listing will update itself?
        }
    }
}
