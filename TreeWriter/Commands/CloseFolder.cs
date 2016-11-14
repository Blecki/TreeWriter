using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CloseFolder : ICommand
    {
        FolderDocument Project;
        public bool Succeeded { get; private set; }

        public CloseFolder(FolderDocument Project)
        {
            this.Project = Project;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            Model.CloseDocument(Project);
        }
    }
}
