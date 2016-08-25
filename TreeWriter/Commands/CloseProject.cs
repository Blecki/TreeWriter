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

        public CloseProject(Project Project)
        {
            this.Project = Project;
        }

        public void Execute(Model Model, Main View)
        {
            Model.CloseProject(Project);
        }
    }
}
