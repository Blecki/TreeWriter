using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class WordParser
    {
        private static String NonWordCharacters = " \t\r\n.,;:\\/\"?![]{}()<>#-";

        private static int FindWordStart(String Text, int Start)
        {
            while (Start < Text.Length && NonWordCharacters.Contains(Text[Start]))
                Start += 1;
            return Start;
        }

        private static int FindWordEnd(String Text, int Start)
        {
            while (Start < Text.Length && !NonWordCharacters.Contains(Text[Start]))
                Start += 1;
            return Start;
        }

        public static IEnumerable<String> EnumerateWords(String Text)
        {
            var start = FindWordStart(Text, 0);
            while (start < Text.Length)
            {
                var end = FindWordEnd(Text, start);
                yield return Text.Substring(start, end - start);
                start = FindWordStart(Text, end);
            }
        }

        public static int CountWords(String Text)
        {
            var start = FindWordStart(Text, 0);
            var result = 0;
            while (start < Text.Length)
            {
                var end = FindWordEnd(Text, start);
                result += 1;
                start = FindWordStart(Text, end);
            }
            return result;
        }
    }
}
