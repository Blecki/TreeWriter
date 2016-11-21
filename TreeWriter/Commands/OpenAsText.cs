using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenAsText : IOpenCommand
    {
        private String FileName;
        private bool SuppressViews = false;
        public EditableDocument Document { get; set; }
        public bool Succeeded { get; private set; }

        public OpenAsText(String FileName, bool SuppressViews)
        {
            this.FileName = FileName;
            this.SuppressViews = SuppressViews;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            try
            {
                Document = Model.FindOpenDocument(FileName) as TextDocument;

                if (Document == null)
                {                   
                    Document = new TextDocument(FileName);
                    if (!SuppressViews) Model.OpenDocument(Document);
                }

                if (SuppressViews)
                {
                    Succeeded = true;
                    return;
                }

                if (Document.OpenEditors.Count != 0)
                    Document.OpenEditors[0].BringToFront();
                else
                    View.OpenControllerPanel(Document.OpenView(Model), WeifenLuo.WinFormsUI.Docking.DockState.Document);

                Succeeded = true;
            }
            catch (Exception)
            {
                Succeeded = false;
            }
        }
    }
}
