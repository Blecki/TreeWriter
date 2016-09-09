using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CloseProject : ICommand
    {
        Project Project;
        public bool Succeeded { get; private set; }

        public CloseProject(Project Project)
        {
            this.Project = Project;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            Model.CloseProject(Project);
        }
    }
}
