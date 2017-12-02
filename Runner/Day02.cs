using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day02 :  Day
    {
        public override string First(string input)
        {
            var lines = input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return lines.Sum(l => GetRowFirstChecksum(l)).ToString();
        }

        public override string Second(string input)
        {
            var lines = input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return lines.Sum(l => GetRowSecondChecksum(l)).ToString();
        }

        public int GetRowFirstChecksum(string input)
        {
            int min = int.MaxValue, max = int.MinValue;
            var numbers = input.Split('\t').Select(s => int.Parse(s));
            foreach (var n in numbers)
            {
                min = Math.Min(n, min);
                max = Math.Max(n, max);
            }
            return (max - min);
        }

        public int GetRowSecondChecksum(string input)
        {
            var numbers = input.Split('\t').Select(s => int.Parse(s)).ToArray();
            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = i + 1; j < numbers.Length; j++)
                {
                    int m = numbers[i], n = numbers[j];
                    if (m % n == 0)
                    {
                        return m / n;
                    }
                    if (n % m == 0)
                    {
                        return n / m;
                    }
                }
            }
            throw new InvalidOperationException("nothing found");
        }

        public override string FirstTest(string input)
        {
            return GetRowFirstChecksum(input).ToString();
        }

        public override string SecondTest(string input)
        {
            return GetRowSecondChecksum(input).ToString();
        }
    }
}
