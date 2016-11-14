using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenDocument : ICommand
    {
        private String FileName;
        public bool Succeeded { get; private set; }

        public OpenDocument(String FileName)
        {
            this.FileName = FileName;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var document = Model.FindOpenDocument(FileName);

            if (document == null)
            {
                var extension = System.IO.Path.GetExtension(FileName);
                if (extension == ".txt")
                    document = new TextDocument
                    {
                        Path = FileName,
                        Contents = System.IO.File.ReadAllText(FileName)
                    };
                else if (extension == ".ms")
                    document = new ManuscriptDocument
                    {
                        Path = FileName
                    };

                Model.OpenDocument(document);
            }

            if (document.OpenEditors.Count != 0)
                document.OpenEditors[0].BringToFront();
            else
                View.OpenControllerPanel(document.OpenView(Model), WeifenLuo.WinFormsUI.Docking.DockState.Document);

            Succeeded = true;
        }
    }
}
