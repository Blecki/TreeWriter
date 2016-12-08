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
            var scrap = String.Format("\nScrapped {0}\nName: {1}\nTags: {2}\nSummary: {3}\n\n",
                   DateTime.Now, Scene.Name, Scene.Tags, Scene.Prose);
            var scrapCommand = new Commands.SendToScrap(scrap, Document.Path);
            scrapCommand.Execute(Model, View);
            if (!scrapCommand.Succeeded) return;

            Document.Data.Scenes.Remove(Scene);
            Document.UpdateViews();
            Succeeded = true;
        }
    }
}
