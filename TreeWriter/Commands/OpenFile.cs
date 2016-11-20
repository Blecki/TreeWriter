using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenFile : ICommand
    {
        private String FileName;
        public bool Succeeded { get; private set; }

        public OpenFile(String FileName)
        {
            this.FileName = FileName;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var extension = System.IO.Path.GetExtension(FileName);
            ICommand realCommand = null;

            if (extension == ".txt")
                realCommand = new OpenAsText(FileName);
            else if (extension == ".ms")
                realCommand = new OpenManuscript(FileName);
            else
                throw new InvalidOperationException("Unknown file type");

            realCommand.Execute(Model, View);
            Succeeded = realCommand.Succeeded;
        }
    }
}
