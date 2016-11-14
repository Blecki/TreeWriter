using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class FolderDocument : EditableDocument
    {
        public override Model.SerializableDocument GetSerializableDocument()
        {
            return new Model.SerializableDocument
            {
                Path = Path,
                Type = "FOLDER"
            };
        }

        public override ControllerPanel OpenView(Model Model)
        {
            var r = new DirectoryListing(this);
            OpenEditors.Add(r);
            return r;
        }
    }
}
