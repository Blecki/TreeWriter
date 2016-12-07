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
            var oldScenePath = ManuscriptDocument.Path + "&" + Scene.Name + ".$prose";
            var newScenePath = ManuscriptDocument.Path + "&" + NewName + ".$prose";

            Scene.Name = NewName;
            ManuscriptDocument.MadeChanges();

            foreach (var openEditor in ManuscriptDocument.OpenEditors)
                if (openEditor is ManuscriptDocumentEditor)
                    (openEditor as ManuscriptDocumentEditor).RebuildLineItem(Scene);
            ManuscriptDocument.UpdateViewTitles();

            //Find any open scenes that match and update their paths.
            foreach (var openScene in Model.FindChildDocuments(oldScenePath))
            {
                if (openScene is SceneDocument)
                {
                    openScene.Path = newScenePath;
                    openScene.UpdateViewTitles();
                }
            }

            Succeeded = true;
        }
    }
}
