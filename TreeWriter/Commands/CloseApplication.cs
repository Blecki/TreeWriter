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
            var needSaveCount = Model.EnumerateOpenDocuments().Count(d => d.HasUnsavedChanges);
            if (needSaveCount > 0)
            {
                var promptResult = System.Windows.Forms.MessageBox.Show("One or more files have changes to save. Save them?", "Alert!", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                if (promptResult == System.Windows.Forms.DialogResult.Yes)
                    foreach (var document in Model.EnumerateOpenDocuments())
                        document.Save(Settings.GlobalSettings.BackupOnSave);
                else if (promptResult == System.Windows.Forms.DialogResult.No)
                {
                    Cancel = false;
                }
                else
                    Cancel = true;
            }

            Succeeded = !Cancel;
        }
    }
}
