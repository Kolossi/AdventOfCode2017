using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day04 :  Day
    {
        public override string First(string input)
        {
            return GetLines(input).Sum(l => FirstIsValid(l)).ToString();
        }

        public override string FirstTest(string input)
        {
            return FirstIsValid(input).ToString();
        }

        public override string SecondTest(string input)
        {
            return SecondIsValid(input).ToString();
        }

        public int FirstIsValid(string line)
        {
            var previousWords = new HashSet<string>();
            foreach (var word in GetParts(line))
            {
                if (previousWords.Contains(word)) return 0;
                previousWords.Add(word);
            }
            return 1;
        }

        public int SecondIsValid(string line)
        {
            var previousWords = new HashSet<string>();
            foreach (var word in GetParts(line))
            {
                foreach (var previousWord in previousWords)
                {
                    if (IsAnagram(word, previousWord)) return 0;
                }
                previousWords.Add(word);
            }
            return 1;
        }

        public static bool IsAnagram(string word1, string word2)
        {
            if (word1.Length != word2.Length) return false;
            var targetChars = new List<char>(word1.ToCharArray());
            foreach (var c in word2)
            {
                bool found = false;
                for (int i = 0; i < targetChars.Count; i++)
                {
                    if (targetChars[i]==c)
                    {
                        targetChars.RemoveAt(i);
                        found = true;
                        break;
                    }
                }
                if (!found) return false;
            }
            return true;
        }

        public override string Second(string input)
        {
            return GetLines(input).Sum(l => SecondIsValid(l)).ToString();
        }
    }
}
