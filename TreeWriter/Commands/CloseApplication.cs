using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CloseApplication : ICommand
    {
        public bool Cancel = false;
        public bool Succeeded { get; private set; }

        public CloseApplication()
        {
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var needSaveCount = Model.EnumerateOpenDocuments().Count(d => d.NeedChangesSaved);
            if (needSaveCount > 0)
            {
                var promptResult = System.Windows.Forms.MessageBox.Show("One or more files have changes to save. Save them?", "Alert!", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                if (promptResult == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (var document in Model.EnumerateOpenDocuments().Where(d => d.NeedChangesSaved))
                        document.SaveDocument();
                }
                else if (promptResult == System.Windows.Forms.DialogResult.No)
                {
                    foreach (var document in Model.EnumerateOpenDocuments()) document.NeedChangesSaved = false; //LIES
                }
                else
                    Cancel = true;
            }

            Succeeded = !Cancel;
        }
    }
}
