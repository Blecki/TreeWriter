using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class DeleteScene :ICommand
    {
        private SceneDocument Scene;
        public bool Succeeded { get; private set; }

        public DeleteScene(SceneDocument Scene)
        {
            this.Scene = Scene;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            var manuscript = Scene.GetRootDocument() as ManuscriptDocument;
            manuscript.SendSceneToScrap(Model, Scene.Data);
            manuscript.Data.Scenes.Remove(Scene.Data);
            manuscript.UpdateViews();
        }
    }
}
