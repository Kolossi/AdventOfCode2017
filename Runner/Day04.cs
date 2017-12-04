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
            return GetLines(input).Sum(l => IsValid(l)).ToString();
        }

        public override string FirstTest(string input)
        {
            return IsValid(input).ToString();
        }

        public int IsValid(string line)
        {
            var used = new HashSet<string>();
            foreach (var word in GetParts(line))
            {
                if (used.Contains(word)) return 0;
                used.Add(word);
            }
            return 1;
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
