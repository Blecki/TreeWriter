using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenScene : ICommand
    {
        private SceneData Scene;
        private ManuscriptDocument Owner;
        public bool Succeeded { get; private set; }

        public OpenScene(SceneData Scene, ManuscriptDocument Owner)
        {
            this.Scene = Scene;
            this.Owner = Owner;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var fakePath = Owner.Path + "&" + Scene.Name;
            var document = Model.FindOpenDocument(fakePath);

            if (document == null)
            {
                document = new SceneDocument
                {
                    Data = Scene,
                    Owner = Owner,
                    Path = fakePath
                };

                Model.OpenDocument(document);
            }

            if (document.OpenEditors.Count != 0)
                document.OpenEditors[0].BringToFront();
            else
                View.OpenControllerPanel(document.OpenView(Model), WeifenLuo.WinFormsUI.Docking.DockState.Document);

            Succeeded = true;
        }
    }
}
