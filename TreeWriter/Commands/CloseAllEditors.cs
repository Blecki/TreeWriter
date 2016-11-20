using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CloseAllEditors : ICommand
    {
        public bool Succeeded { get; private set; }

        public CloseAllEditors()
        {
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var cancel = false;
            var needSaveCount = Model.EnumerateOpenDocuments().Count(d => d.HasUnsavedChanges);
            if (needSaveCount > 0)
            {
                var promptResult = System.Windows.Forms.MessageBox.Show("One or more files have changes to save. Save them?", "Alert!", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                if (promptResult == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (var document in Model.EnumerateOpenDocuments().Where(d => d.HasUnsavedChanges))
                        document.SaveDocument();
                }
                else if (promptResult == System.Windows.Forms.DialogResult.No)
                {
                    cancel = false;
                }
                else
                    cancel = true;
            }

            if (!cancel)
            {
                var documents = new List<EditableDocument>(Model.EnumerateOpenDocuments());
                foreach (var document in documents)
                {
                    document.CloseAllViews(EditableDocument.CloseStyle.ForcedWithoutSaving);
                    Model.CloseDocument(document);
                }
            }

            Succeeded = !cancel;
        }
    }
}
