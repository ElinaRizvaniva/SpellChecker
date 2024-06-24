using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellChecker
{
    public class SpellingCorrection
    {
        public HashSet<string> DictionaryWords { get; }

        public SpellingCorrection(string dictionaryText)
        {
            string[] dictionary = dictionaryText
                .Split(new string[] { "\r\n", " " }, StringSplitOptions.RemoveEmptyEntries)
                .Where(word => word.Length <= 50)
                .ToArray();
            if (dictionary.Length == 0) throw new DictionaryException("Empty dictionary");
            DictionaryWords = new HashSet<string>(dictionary, StringComparer.OrdinalIgnoreCase);
        }

        public string[] CorrectTextLines(string[] textLines)
        {
            //TODO Добавить проверку
            return textLines;
        }

        public class DictionaryException : Exception
        {
            public DictionaryException(string message) : base(message) { }
        }
    }
}