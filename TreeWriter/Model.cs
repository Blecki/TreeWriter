using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TreeWriterWF
{
    public class Model
    {
        public List<Document> OpenDocuments = new List<Document>();
        public List<Project> OpenProjects = new List<Project>();

        public class SerializableDocument
        {
            public String Path;
            public String Project;
        }

        public class SerializableSettings
        {
            public List<SerializableDocument> OpenDocuments;
            public List<String> OpenProjects;
        }

        public void LoadSettings(Main View)
        {
            try
            {
                var settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BleckiTreeWriter\\settings.txt";
                if (System.IO.File.Exists(settingsPath))
                {
                    var text = System.IO.File.ReadAllText(settingsPath);
                    var settingsObject = JsonConvert.DeserializeObject<SerializableSettings>(text);

                    foreach (var folder in settingsObject.OpenProjects)
                        View.ProcessControllerCommand(new Commands.OpenProject(folder));

                    foreach (var document in settingsObject.OpenDocuments)
                    {
                        var owner = OpenProjects.FirstOrDefault(p => p.Path == document.Project);
                        if (owner != null)
                            View.ProcessControllerCommand(new Commands.OpenFile(document.Path, owner));
                    }

                    
                }

            } catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error loading settings.", "Alert!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        public void SaveSettings()
        {
            try
            {
                var settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BleckiTreeWriter";
                if (!System.IO.Directory.Exists(settingsDirectory))
                    System.IO.Directory.CreateDirectory(settingsDirectory);
                var settingsPath = settingsDirectory + "\\settings.txt";

                var settingsObject = new SerializableSettings
                {
                    OpenDocuments = OpenDocuments.Select(d => new SerializableDocument
                    {
                        Path = d.FileName,
                        Project = d.Owner.Path
                    }).ToList(),
                    OpenProjects = OpenProjects.Select(d => d.Path).ToList()
                };
                
                System.IO.File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settingsObject));
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error saving settings.", "Alert!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
        
        public Document FindOpenDocument(String FileName)
        {
            return OpenDocuments.FirstOrDefault(d => d.FileName == FileName);
        }

        public Document OpenDocument(String FileName, Project Owner)
        {
            var existingDocument = FindOpenDocument(FileName);
            if (existingDocument == null)
            {
                existingDocument = new Document();
                existingDocument.FileName = FileName;
                existingDocument.Contents = System.IO.File.ReadAllText(FileName);
                existingDocument.Owner = Owner;
                OpenDocuments.Add(existingDocument);
            }
            return existingDocument;
        }

        public Project OpenProject(String ProjectPath)
        {
            var existingProject = OpenProjects.FirstOrDefault(d => d.Path == ProjectPath);
            if (existingProject == null)
            {
                existingProject = new Project
                {
                    Path = ProjectPath,
                    OpenView = null
                };
                OpenProjects.Add(existingProject);
            }
            return existingProject;
        }

        public void CloseProject(Project Project)
        {
            OpenProjects.Remove(Project);
        }

        /// <summary>
        /// Close an open document.
        /// DocumentEditors will be orphaned. 
        /// </summary>
        /// <param name="Document"></param>
        public void CloseDocument(Document Document)
        {
            System.Diagnostics.Debug.Assert(Document.OpenEditors.Count <= 1);
            OpenDocuments.Remove(Document);
        }

        public void SaveDocument(Document Document)
        {
            System.IO.File.WriteAllText(Document.FileName, Document.Contents);
        }

        public String WikiSearch(String ProjectDirectory, String ArticleName)
        {
            var localFile = ProjectDirectory + "\\" + ArticleName + ".txt";
            if (System.IO.File.Exists(localFile)) return localFile;
            else foreach (var directory in System.IO.Directory.EnumerateDirectories(ProjectDirectory))
                {
                    var r = WikiSearch(directory, ArticleName);
                    if (!String.IsNullOrEmpty(r)) return r;
                }
            return null;
        }
    }
}
