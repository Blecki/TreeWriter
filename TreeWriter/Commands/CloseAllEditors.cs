using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CloseAllEditors : ICommand
    {
        public void Execute(Model Model, Main View)
        {
            var cancel = false;
            var needSaveCount = Model.OpenDocuments.Count(d => d.NeedChangesSaved);
            if (needSaveCount > 0)
            {
                var promptResult = System.Windows.Forms.MessageBox.Show("One or more files have changes to save. Save them?", "Alert!", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                if (promptResult == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (var document in Model.OpenDocuments.Where(d => d.NeedChangesSaved))
                        Model.SaveDocument(document);
                }
                else if (promptResult == System.Windows.Forms.DialogResult.No)
                {
                    foreach (var document in Model.OpenDocuments) document.NeedChangesSaved = false; //LIES
                }
                else
                    cancel = true;
            }

            if (!cancel)
            {
                var documents = new List<Document>(Model.OpenDocuments);
                foreach (var document in documents)
                {
                    var editors = new List<DocumentEditor>(document.OpenEditors);
                    foreach (var editor in editors)
                        editor.Close();
                }
            }
        }
    }
}
