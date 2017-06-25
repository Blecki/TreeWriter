using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class FileMetaInformation
    {
        public int WordCount = 0;
    }

    public class DirectoryMetaInformation
    {
        public int TotalWordCount = 0;
        public Dictionary<String, FileMetaInformation> Files = new Dictionary<string, FileMetaInformation>();
    }

    public class MetaInformation
    {
        public DirectoryMetaInformation Data;
        public String Path;

        public MetaInformation(String Path)
        {
            this.Path = Path;

            var metaFileName = Path + "\\" + ".meta";

            if (System.IO.File.Exists(metaFileName))
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject<DirectoryMetaInformation>(
                    System.IO.File.ReadAllText(metaFileName));
            else
                Data = new DirectoryMetaInformation();
        }

        public void Save()
        {
            var metaFileName = Path + "\\" + ".meta";
            System.IO.File.WriteAllText(metaFileName, Newtonsoft.Json.JsonConvert.SerializeObject(Data));
        }

        public void UpdateFromDisc()
        {
            Data = new DirectoryMetaInformation();

            foreach (var directory in Model.EnumerateDirectories(Path))
                Data.TotalWordCount += (new MetaInformation(directory)).Data.TotalWordCount;

            foreach (var file in Model.EnumerateFiles(Path))
                Data.Files.Add(file, new FileMetaInformation
                    {
                        WordCount = WordParser.CountWords(System.IO.File.ReadAllText(file))
                    });

            Data.TotalWordCount += Data.Files.Select(f => f.Value.WordCount).Sum();
        }
    }
}
