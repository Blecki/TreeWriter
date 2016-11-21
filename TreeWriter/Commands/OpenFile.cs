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
        private bool SuppressViews = false;
        internal EditableDocument Document;
        public bool Succeeded { get; private set; }

        public OpenFile(String FileName, bool SuppressViews = false)
        {
            this.FileName = FileName;
            this.SuppressViews = SuppressViews;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var extension = System.IO.Path.GetExtension(FileName);
            IOpenCommand realCommand = null;

            if (extension == ".txt")
                realCommand = new OpenAsText(FileName, SuppressViews);
            else if (extension == ".ms")
                realCommand = new OpenManuscript(FileName, SuppressViews);
            else
                throw new InvalidOperationException("Unknown file type");

            realCommand.Execute(Model, View);
            Succeeded = realCommand.Succeeded;
            Document = realCommand.Document;
        }
    }
}
