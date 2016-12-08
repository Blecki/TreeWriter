using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class EditableDocument
    {
        public String Path;
        protected bool NeedChangesSaved = false;
        public List<DockablePanel> OpenEditors = new List<DockablePanel>();

        public bool HasUnsavedChanges { get { return NeedChangesSaved; } }

        public virtual void Load(Model Model, Main View, String Path)
        {
            throw new NotImplementedException();
        }
        
        public virtual void ApplyChanges(String NewText)
        {
            NeedChangesSaved = true;
            // If editors are properly linked through scintilla documents, they should already share changes.
            foreach (var editor in OpenEditors) editor.Text = this.GetTitle();
        }

        public virtual void MadeChanges()
        {
            NeedChangesSaved = true;
            UpdateViewTitles();
        }

        public virtual String GetContents() { throw new NotImplementedException(); }

        public virtual int CountWords(Model Model, Main View) { throw new NotImplementedException(); }
        
        public String GetTitle()
        {
            return (NeedChangesSaved ? "*" : "") + ImplementGetEditorTitle();
        }

        protected virtual String ImplementGetEditorTitle()
        {
            throw new NotImplementedException();
        }

        public virtual void Save(bool Backup) { }

        protected String GetBackupFilename()
        {
            var directory = System.IO.Path.GetDirectoryName(Path);
            var file = System.IO.Path.GetFileName(Path);
            var newDirectory = directory + "\\" + file.Replace('.', '_');
            if (!System.IO.Directory.Exists(newDirectory))
                System.IO.Directory.CreateDirectory(newDirectory);
            var newPath = newDirectory + "\\" + System.Guid.NewGuid();
            return newPath;
        }

        public enum CloseStyle
        {
            ForcedWithoutSaving,
            Natural
        }

        public void CloseAllViews(CloseStyle CloseStyle = CloseStyle.Natural)
        {
            var editors = new List<DockablePanel>(OpenEditors);
            foreach (var editor in editors)
            {
                editor.CloseStyle = CloseStyle;
                editor.Close();
            }
        }

        public virtual void Close()
        { }

        public void ClearChangesFlag()
        {
            NeedChangesSaved = false;
        }

        public virtual DockablePanel OpenView(Model Model)
        {
            throw new NotImplementedException();
        }

        public virtual WeifenLuo.WinFormsUI.Docking.DockState GetPreferredOpeningDockState()
        {
            return WeifenLuo.WinFormsUI.Docking.DockState.Document;
        }

        public void UpdateViews()
        {
            foreach (var editor in OpenEditors)
            {
                editor.ReloadDocument();
                editor.Text = GetTitle();
            }
        }

        public void UpdateViewTitles()
        {
            foreach (var editor in OpenEditors)
                editor.Text = GetTitle();
        }
    }
}
