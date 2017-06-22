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
        private List<EditableDocument> OpenDocuments = new List<EditableDocument>();

        public NHunspell.Hunspell SpellChecker { get; private set; }
        public NHunspell.MyThes Thesaurus;

        public void LoadSettings(Main View)
        {
            try
            {
                var settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BleckiTreeWriter\\settings.txt";

                if (System.IO.File.Exists(settingsPath))
                {
                    var text = System.IO.File.ReadAllText(settingsPath);
                    Settings.GlobalSettings = JsonConvert.DeserializeObject<Settings>(text);
                }

                
            }
            catch (Exception e)
            {
                SpellChecker = new NHunspell.Hunspell("en_US.aff", "en_US.dic");
                Thesaurus = new NHunspell.MyThes("th_en_US_new.dat");

                System.Windows.Forms.MessageBox.Show("Error loading settings.", "Alert!", System.Windows.Forms.MessageBoxButtons.OK);
            }

            if (Settings.GlobalSettings == null) Settings.GlobalSettings = new Settings();

            Settings.GlobalSettings.Verify();

            SpellChecker = new NHunspell.Hunspell(
                Settings.GlobalSettings.Dictionary + ".aff",
                Settings.GlobalSettings.Dictionary + ".dic");
            Thesaurus = new NHunspell.MyThes(Settings.GlobalSettings.Thesaurus);

            foreach (var word in Settings.GlobalSettings.CustomDictionaryEntries)
                SpellChecker.Add(word);


            View.UpdateRecentDocuments();
        }

        public void SaveSettings()
        {
            try
            {
                var settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BleckiTreeWriter";
                if (!System.IO.Directory.Exists(settingsDirectory))
                    System.IO.Directory.CreateDirectory(settingsDirectory);
                var settingsPath = settingsDirectory + "\\settings.txt";

                System.IO.File.WriteAllText(settingsPath, JsonConvert.SerializeObject(Settings.GlobalSettings, Formatting.Indented));
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error saving settings.", "Alert!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
        
        public EditableDocument FindOpenDocument(String FileName)
        {
            return OpenDocuments.FirstOrDefault(d => d.Path.ToUpper() == FileName.ToUpper());
        }

        public IEnumerable<EditableDocument> FindChildDocuments(String BaseFileName)
        {
            foreach (var document in OpenDocuments)
                if (document.Path.ToUpper().StartsWith(BaseFileName.ToUpper())) yield return document;
        }

        /// <summary>
        /// Close an open document.
        /// DocumentEditors will be orphaned. 
        /// </summary>
        /// <param name="Document"></param>
        public void CloseDocument(EditableDocument Document)
        {
            System.Diagnostics.Debug.Assert(Document.OpenEditors.Count <= 1);
            OpenDocuments.Remove(Document);
            Document.Close();
        }

        public void OpenDocument(EditableDocument Document)
        {
            OpenDocuments.Add(Document);
        }

        public IEnumerable<EditableDocument> EnumerateOpenDocuments()
        {
            return OpenDocuments;
        }

        public static IEnumerable<String> EnumerateDirectories(String Path)
        {
            foreach (var subDir in System.IO.Directory.EnumerateDirectories(Path))
            {
                var lastDirectory = System.IO.Path.GetFileName(subDir);
                if (!lastDirectory.StartsWith("backup--"))
                    yield return subDir;
            }
        }

        public static IEnumerable<String> EnumerateFiles(String Path)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Path))
            {
                var fileName = System.IO.Path.GetFileName(file);
                var extension = System.IO.Path.GetExtension(file);
                if (extension == ".txt")
                    yield return file;
            }
        }
    }
}
