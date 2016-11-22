using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public abstract class OpenCommand : ICommand
    {
        public enum OpenStyles
        {
            Transient,
            CreateView
        }

        protected String FileName;
        protected OpenStyles OpenStyle = OpenStyles.CreateView;
        public EditableDocument Document { get; protected set; }
        public bool Succeeded { get; protected set; }

        public void Execute(Model Model, Main View)
        {
            ImplementExecute(Model, View);
        }

        protected virtual void ImplementExecute(Model Model, Main View)
        {
            throw new NotImplementedException();
        }

    }

    public class OpenCommand<T> : OpenCommand where T: EditableDocument, new()
    {
        public OpenCommand(String FileName, OpenStyles OpenStyle)
        {
            this.FileName = FileName;
            this.OpenStyle = OpenStyle;
            this.Succeeded = false;
            this.Document = null;
        }

        protected override void ImplementExecute(Model Model, Main View)
        {
            try
            {
                Document = Model.FindOpenDocument(FileName) as T;

                if (Document == null)
                {
                    Document = new T();
                    Document.Load(FileName);
                    if (this.OpenStyle == OpenCommand.OpenStyles.CreateView) Model.OpenDocument(Document);
                }

                if (this.OpenStyle == OpenCommand.OpenStyles.Transient)
                {
                    Succeeded = true;
                    return;
                }

                if (Document.OpenEditors.Count != 0)
                    Document.OpenEditors[0].BringToFront();
                else
                    View.OpenControllerPanel(Document.OpenView(Model), WeifenLuo.WinFormsUI.Docking.DockState.Document);

                Succeeded = true;
            }
            catch (Exception)
            {
                Succeeded = false;
            }
        }
    }
}
