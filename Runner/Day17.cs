using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day17 : Day
    {
        List<int> Buffer = new List<int>();

        public int SpinBuffer(int skipLength)
        {
            int pos = 0;
            Buffer = new List<int>() { 0 };
            for (int i = 1; i <= 2017; i++)
            {
                pos = (pos + skipLength) % (i);
                if (pos==i-1)
                {
                    Buffer.Add(i);
                }
                else
                {
                    Buffer.Insert(pos+1, i);
                }
                pos = (pos + 1) % (i + 1);
                //Console.WriteLine(string.Join("", Buffer.Select((b, j) => j == pos 
                //                                            ? string.Format("({0})", b)
                //                                            : string.Format(" {0} ", b))));
            }

            return Buffer[(pos + 1) % Buffer.Count];
        }

        public int FindZeroFollower(int skipLength, int spinLength = 2018)
        {
            int pos = 0;
            int zeroFollower = 0;
            for (int i = 1; i <= spinLength; i++)
            {
                pos = (pos + skipLength) % (i);
                if (pos == 0)
                {
                    zeroFollower = i;
                }
                pos = (pos + 1) % (i + 1);
                if (i % 1000000 == 0) Console.Write(".");
            }

            return zeroFollower;
        }

        public override string First(string input)
        {
            var skipLength = int.Parse(input);
            var next = SpinBuffer(skipLength);
            return next.ToString();
        }

        public override string Second(string input)
        {
            var parts = GetParts(input);
            var skipLength = int.Parse(parts[0]);
            var spinLength = int.Parse(parts[1]);
            return FindZeroFollower(skipLength, spinLength).ToString();
        }
    }
}
