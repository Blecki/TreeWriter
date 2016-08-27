using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class AddWordToDictionary :ICommand
    {
        private String Word;
        
        public AddWordToDictionary(String Word)
        {
            this.Word = Word;
        }

        public void Execute(Model Model, Main View)
        {
            Model.CustomDictionaryEntries.Add(Word);
            Model.SpellChecker.Add(Word);
        }
    }
}
