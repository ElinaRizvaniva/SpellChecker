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
            for (int textLinePosition = 0; textLinePosition < textLines.Length; textLinePosition++)
            {
                string[] words = textLines[textLinePosition].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int wordPosition = 0; wordPosition < words.Length; wordPosition++)
                {
                    if (words[wordPosition].Length <= 50)
                        words[wordPosition] = CorrectWord(words[wordPosition]);
                }
                textLines[textLinePosition] = string.Join(" ", words);
            }
            return textLines;
        }

        private string CorrectWord(string word)
        {
            if (!DictionaryWords.Contains(word))
            {
                List<string> corrections = GetCorrections(word);
                switch (corrections.Count)
                {
                    case 0:
                        word = "{" + word + "?}";
                        break;
                    case 1:
                        word = corrections[0];
                        break;
                    default:
                        word = "{" + string.Join(" ", corrections) + "}";
                        break;
                }
            }
            return word;
        }

        private List<string> GetCorrections(string word)
        {
            List<string> correctionsWithOneEdit = new List<string>();
            List<string> correctionsWithTwoEdit = new List<string>();
            List<string> filtredDictionaryWords = FilterDictionary(word);

            foreach (string dictionaryWord in filtredDictionaryWords)
            {
                int sizeDifference = dictionaryWord.Length - word.Length;
                if (correctionsWithOneEdit.Count > 0 && !(Math.Abs(sizeDifference) == 1))
                    continue;

                string wordToEdit = word;
                string comparedWord = dictionaryWord;
                if (sizeDifference > 0)
                {
                    wordToEdit = dictionaryWord;
                    comparedWord = word;
                }
                else sizeDifference = Math.Abs(sizeDifference);

                for (int position = 0; position < wordToEdit.Length; position++)
                {
                    string wordWithDeletedLetter;
                    if (sizeDifference == 2)
                    {
                        if (position < wordToEdit.Length - 2)
                            wordWithDeletedLetter = wordToEdit
                                .Remove(position, 1)
                                .Remove(position + 1, 1);
                        else break;
                    }
                    else
                    {
                        wordWithDeletedLetter = wordToEdit.Remove(position, 1);

                        if (sizeDifference == 0)
                            comparedWord = EditComparedWord(comparedWord, wordWithDeletedLetter);
                    }

                    if (string.Equals(wordWithDeletedLetter, comparedWord, StringComparison.OrdinalIgnoreCase))
                    {
                        if (sizeDifference == 1)
                            correctionsWithOneEdit.Add(dictionaryWord);
                        else
                            correctionsWithTwoEdit.Add(dictionaryWord);
                        break;
                    }
                }
            }
            return correctionsWithOneEdit.Count > 0 ? correctionsWithOneEdit : correctionsWithTwoEdit;
        }

        private List<string> FilterDictionary(string word)
        {
            List<string> filteredDictionaryWords = new List<string>();
            char[] wordForFilter = word.ToLower().Distinct().ToArray();
            foreach (string dictionaryWord in DictionaryWords)
            {
                char[] dictionaryWordForFilter = dictionaryWord.ToLower().Distinct().ToArray();
                List<char> letterIntersection = wordForFilter.Intersect(dictionaryWordForFilter).ToList();

                int num = dictionaryWord.Length - word.Length;
                switch (num)
                {
                    case -2:
                    case -1:
                        if (letterIntersection.Count == dictionaryWordForFilter.Length)
                            filteredDictionaryWords.Add(dictionaryWord);
                        break;
                    case 0:
                        if (wordForFilter.Length - letterIntersection.Count <= 1)
                            filteredDictionaryWords.Add(dictionaryWord);
                        break;
                    case 1:
                    case 2:
                        if (dictionaryWordForFilter.Length - letterIntersection.Count <= num)
                            filteredDictionaryWords.Add(dictionaryWord);
                        break;
                }
            }
            return filteredDictionaryWords;
        }

        private string EditComparedWord(string comparedWord, string wordWithDeletedLetter)
        {
            for (int position = 0; position < comparedWord.Length; position++)
            {
                string comparedWordWithDeletedLetter = comparedWord.Remove(position, 1);
                if (string.Equals(wordWithDeletedLetter, comparedWordWithDeletedLetter, StringComparison.OrdinalIgnoreCase))
                {
                    comparedWord = comparedWordWithDeletedLetter;
                    break;
                }
            }
            return comparedWord;
        }

        public class DictionaryException : Exception
        {
            public DictionaryException(string message) : base(message) { }
        }
    }
}