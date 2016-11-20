using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenAsText : ICommand
    {
        private String FileName;
        public bool Succeeded { get; private set; }

        public OpenAsText(String FileName)
        {
            this.FileName = FileName;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            try
            {
                var document = Model.FindOpenDocument(FileName) as TextDocument;

                if (document == null)
                {                   
                    document = new TextDocument(FileName);
                    Model.OpenDocument(document);
                }

                if (document.OpenEditors.Count != 0)
                    document.OpenEditors[0].BringToFront();
                else
                    View.OpenControllerPanel(document.OpenView(Model), WeifenLuo.WinFormsUI.Docking.DockState.Document);

                Succeeded = true;
            }
            catch (Exception)
            {
                Succeeded = false;
            }
        }
    }
}
