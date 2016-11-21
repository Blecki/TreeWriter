using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenManuscript : IOpenCommand
    {
        private String FileName;
        private bool SuppressViews;
        public EditableDocument Document { get; set; }
        public bool Succeeded { get; private set; }

        public OpenManuscript(String FileName, bool SuppressViews)
        {
            this.FileName = FileName;
            this.SuppressViews = SuppressViews;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            try
            {
                Document = Model.FindOpenDocument(FileName);

                if (Document == null)
                {
                    var json = System.IO.File.ReadAllText(FileName);

                    if (String.IsNullOrEmpty(json))
                        Document = new ManuscriptDocument(FileName, ManuscriptData.CreateBlank());
                    else
                        Document = new ManuscriptDocument(FileName, ManuscriptData.CreateFromJson(json));

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
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Open operation has been aborted to avoid loss of data. Fail reason: " + e.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK);
                Succeeded = false;
            }
        }
    }
}
