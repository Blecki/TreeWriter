using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class DeleteScene :ICommand
    {
        private SceneData Scene;
        private ManuscriptDocument Document;
        public bool Succeeded { get; private set; }

        public DeleteScene(ManuscriptDocument Document, SceneData Scene)
        {
            this.Document = Document;
            this.Scene = Scene;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            Document.SendSceneToScrap(Model, Scene);
            Document.Data.Scenes.Remove(Scene);
            Document.UpdateViews();
        }
    }
}
