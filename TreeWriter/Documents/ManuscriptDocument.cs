using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class ManuscriptDocument : EditableDocument
    {
        public List<SceneDocument> Scenes = new List<SceneDocument>();

        public override Model.SerializableDocument GetSerializableDocument()
        {
            return new Model.SerializableDocument
            {
                Path = Path,
                Type = "MANUSCRIPT"
            };
        }

        public override ControllerPanel OpenView(Model Model)
        {
            var r = new SceneListing(this);
            OpenEditors.Add(r);
            return r;
        }
    }
}
