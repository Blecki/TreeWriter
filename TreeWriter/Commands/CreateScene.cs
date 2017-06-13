using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CreateScene : ICommand
    {
        private ManuscriptDocument Owner;
        private SceneData InsertAfter;
        public bool Succeeded { get; private set; }
        public SceneData NewScene;

        public CreateScene(SceneData InsertAfter, ManuscriptDocument Owner)
        {
            this.InsertAfter = InsertAfter;
            this.Owner = Owner;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            NewScene = new SceneData
            {
                Name = "New Scene"
            };

            if (InsertAfter != null)
                Owner.Data.Scenes.Insert(Owner.Data.Scenes.IndexOf(InsertAfter) + 1, NewScene);
            else
                Owner.Data.Scenes.Add(NewScene);

            Owner.MadeChanges();
            Succeeded = true;
        }
    }
}
