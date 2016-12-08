using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenPath : ICommand
    {
        private String FileName;
        private OpenCommand.OpenStyles OpenStyle = OpenCommand.OpenStyles.CreateView;
        internal EditableDocument Document;
        internal DockablePanel Panel;
        public bool Succeeded { get; private set; }

        public OpenPath(String FileName, OpenCommand.OpenStyles OpenStyle)
        {
            this.FileName = FileName;
            this.OpenStyle = OpenStyle;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var extension = System.IO.Path.GetExtension(FileName);
            OpenCommand realCommand = null;

            if (extension == ".txt")
                realCommand = new OpenCommand<TextDocument>(FileName, OpenStyle);
            else if (extension == ".ms")
                realCommand = new OpenCommand<ManuscriptDocument>(FileName, OpenStyle);
            else if (extension == ".$prose")
                realCommand = new OpenCommand<SceneDocument>(FileName, OpenStyle);
            else if (extension == ".$settings")
                realCommand = new OpenCommand<SceneSettingsDocument>(FileName, OpenStyle);
            else if (extension == ".$notes")
                realCommand = new OpenCommand<NotesDocument>(FileName, OpenStyle);
            else if (System.IO.Directory.Exists(FileName))
                realCommand = new OpenCommand<FolderDocument>(FileName, OpenStyle);
            else
                throw new InvalidOperationException("Unknown file type");

            realCommand.Execute(Model, View);
            Succeeded = realCommand.Succeeded;
            Document = realCommand.Document;
            Panel = realCommand.Panel;

            if (Succeeded == false && OpenStyle == OpenCommand.OpenStyles.CreateView)
                System.Windows.Forms.MessageBox.Show(String.Format("Opening {0} failed or was aborted to preserve your data. Error message: {1}",
                    FileName, realCommand.ErrorMessage));
        }
    }
}
