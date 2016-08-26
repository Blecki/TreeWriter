using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class SaveOpenDocuments :ICommand
    {
        public void Execute(Model Model, Main View)
        {
            foreach (var document in Model.OpenDocuments)
            {
                if (document.NeedChangesSaved)
                {
                    Model.SaveDocument(document);
                    foreach (var editor in document.OpenEditors) editor.UpdateTitle();
                }
            }
        }
    }
}
