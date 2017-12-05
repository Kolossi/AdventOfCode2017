using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day05 :  Day
    {
        public override string First(string input)
        {
            var jumps = GetLines(input).Select(l => int.Parse(l)).ToArray();
            return FindFirstExitCount(jumps);
        }

        private string FindFirstExitCount(int[] jumps)
        {
            int count = 0, ptr = 0,jump;
            while (ptr<jumps.Length)
            {
                jump = jumps[ptr];
                jumps[ptr]++;
                count++;
                ptr += jump;
            }
            return count.ToString();
        }

        public override string FirstTest(string input)
        {
            return First(string.Join("\r\n", GetParts(input)));
        }

        public override string Second(string input)
        {
            var jumps = GetLines(input).Select(l => int.Parse(l)).ToArray();
            return FindSecondExitCount(jumps);
        }

        private string FindSecondExitCount(int[] jumps)
        {
            int count = 0, ptr = 0, jump;
            while (ptr < jumps.Length)
            {
                jump = jumps[ptr];
                if (jump >= 3)
                {
                    jumps[ptr]--;
                }
                else
                {
                    jumps[ptr]++;
                }
                count++;
                ptr += jump;
            }
            return count.ToString();
        }

        public override string SecondTest(string input)
        {
            return Second(string.Join("\r\n", GetParts(input)));
        }

    }
}
