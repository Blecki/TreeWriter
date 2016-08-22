using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TreeWriterWF
{
    public class ProjectModel
    {
        public List<Document> OpenDocuments = new List<Document>();
        public List<String> OpenDirectories = new List<String>();

        public class SerializableSettings
        {
            public List<String> OpenDocuments;
            public List<String> OpenDirectories;
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

                    foreach (var document in settingsObject.OpenDocuments)
                        View.ProcessControllerCommand(new Commands.OpenFile(document));

                    foreach (var folder in settingsObject.OpenDirectories)
                        View.ProcessControllerCommand(new Commands.OpenDirectory(folder));
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
                    OpenDocuments = OpenDocuments.Select(d => d.FileName).ToList(),
                    OpenDirectories = OpenDirectories
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

        public Document OpenDocument(String FileName)
        {
            var existingDocument = FindOpenDocument(FileName);
            if (existingDocument == null)
            {
                existingDocument = new Document();
                existingDocument.FileName = FileName;
                existingDocument.Contents = System.IO.File.ReadAllText(FileName);
                OpenDocuments.Add(existingDocument);
            }
            return existingDocument;
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


    }
}
