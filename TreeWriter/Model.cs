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
        String DictionaryBase = "en_US";
        public List<String> CustomDictionaryEntries = new List<string>();
        String ThesaurusData = "th_en_US_new.dat";
        public NHunspell.MyThes Thesaurus;

        public class SerializableSettings
        {
            public List<OpenDocumentRecord> OpenDocuments;
            public String Dictionary;
            public String Thesaurus;
            public List<String> CustomDictionaryEntries;
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

                    if (!String.IsNullOrEmpty(settingsObject.Dictionary))
                        DictionaryBase = settingsObject.Dictionary;
        
                    SpellChecker = new NHunspell.Hunspell(DictionaryBase + ".aff", DictionaryBase + ".dic");

                    if (!String.IsNullOrEmpty(settingsObject.Thesaurus))
                        ThesaurusData = settingsObject.Thesaurus;

                    Thesaurus = new NHunspell.MyThes(ThesaurusData);
        
                    foreach (var document in settingsObject.OpenDocuments.Where(d => d.Type == "FOLDER"))
                        View.ProcessControllerCommand(new Commands.OpenFolder(document.Path));

                    // Todo: Properly open documents.
                    foreach (var document in settingsObject.OpenDocuments.Where(d => d.Type != "FOLDER"))
                        View.ProcessControllerCommand(new Commands.OpenFile(document.Path));
                    
                    if (settingsObject.CustomDictionaryEntries != null)
                        foreach (var word in settingsObject.CustomDictionaryEntries)
                        {
                            CustomDictionaryEntries.Add(word);
                            SpellChecker.Add(word);
                        }
                }
                else
                {
                    DictionaryBase = "en_US";
                    SpellChecker = new NHunspell.Hunspell(DictionaryBase + ".aff", DictionaryBase + ".dic");
                    ThesaurusData = "th_en_US_new.dat";
                    Thesaurus = new NHunspell.MyThes(ThesaurusData);
                }
            } 
            catch (Exception e)
            {
                DictionaryBase = "en_US";
                SpellChecker = new NHunspell.Hunspell(DictionaryBase + ".aff", DictionaryBase + ".dic");
                ThesaurusData = "th_en_US_new.dat";                
                Thesaurus = new NHunspell.MyThes(ThesaurusData);
                
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
                    OpenDocuments = OpenDocuments.Select(d => d.GetOpenDocumentRecord()).Where(s => s != null).ToList(),
                    Dictionary = DictionaryBase,
                    CustomDictionaryEntries = CustomDictionaryEntries,
                    Thesaurus = ThesaurusData
                };
                
                System.IO.File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settingsObject, Formatting.Indented));
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
        }

        public void OpenDocument(EditableDocument Document)
        {
            OpenDocuments.Add(Document);
        }

        public IEnumerable<EditableDocument> EnumerateOpenDocuments()
        {
            return OpenDocuments;
        }
    }
}
