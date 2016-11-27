using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CreateNewDocumentAtPath : ICommand
    {
        public String NewFileName;
        public bool Succeeded { get; private set; }

        public CreateNewDocumentAtPath(String NewFileName)
        {
            this.NewFileName = NewFileName;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            if (!System.IO.File.Exists(NewFileName))
            {
                var file = System.IO.File.CreateText(NewFileName);
                file.Close();
            }

            Succeeded = true;
        }
    }
}
