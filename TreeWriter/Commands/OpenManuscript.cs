using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenManuscript : ICommand
    {
        private String FileName;
        public bool Succeeded { get; private set; }

        public OpenManuscript(String FileName)
        {
            this.FileName = FileName;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            try
            {
                var document = Model.FindOpenDocument(FileName);

                if (document == null)
                {
                    var json = System.IO.File.ReadAllText(FileName);

                    if (String.IsNullOrEmpty(json))
                        document = new ManuscriptDocument(FileName, ManuscriptData.CreateBlank());
                    else
                        document = new ManuscriptDocument(FileName, ManuscriptData.CreateFromJson(json));

                    Model.OpenDocument(document);
                }

                if (document.OpenEditors.Count != 0)
                    document.OpenEditors[0].BringToFront();
                else
                    View.OpenControllerPanel(document.OpenView(Model), WeifenLuo.WinFormsUI.Docking.DockState.Document);

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
