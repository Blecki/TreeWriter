using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class RenameScene :ICommand
    {
        private ManuscriptDocument ManuscriptDocument;
        private SceneData Scene;
        private String NewName;
        public bool Succeeded { get; private set; }

        public RenameScene(ManuscriptDocument ManuscriptDocument, SceneData Scene, String NewName)
        {
            this.ManuscriptDocument = ManuscriptDocument;
            this.Scene = Scene;
            this.NewName = NewName;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            Scene.Name = NewName;
            ManuscriptDocument.MadeChanges();
            ManuscriptDocument.UpdateViews();
            Succeeded = true;
        }
    }
}
