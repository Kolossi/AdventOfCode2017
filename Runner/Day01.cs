using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    class Day01 :  Day
    {
        public override string First(string input)
        {
            int offset = input.Length - 1;
            return Solve(input, offset);
        }

        public string RotateStringBy(string input, int n)
        {
            var newString = input.Substring(n, input.Length - n) + input.Substring(0, n);
            return newString;
        }

        public override string Second(string input)
        {
            int offset = input.Length / 2;
            return Solve(input, offset);
        }



        private string Solve(string input, int offset)
        {
            int total = 0;
            var rotated = RotateStringBy(input, offset).ToCharArray();
            var orig = input.ToCharArray();
            for (int i = 0; i < input.Length; i++)
            {
                var c = orig[i];
                if (c == rotated[i])
                {
                    total += int.Parse(c.ToString());
                }
            }
            return total.ToString();
        }
    }
}
