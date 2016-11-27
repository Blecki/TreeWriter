using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class AddWordToDictionary : ICommand
    {
        private String Word;
        public bool Succeeded { get; private set; }
        
        public AddWordToDictionary(String Word)
        {
            this.Word = Word;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            Model.Settings.CustomDictionaryEntries.Add(Word);
            Model.SpellChecker.Add(Word);

            Succeeded = true;
        }
    }
}
