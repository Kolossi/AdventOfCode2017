using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    class Day15 : Day
    {
        public const UInt64 Afactor = 16807;
        public const UInt64 Afilter = 0x3;

        public const UInt64 Bfactor = 48271;
        public const UInt64 Bfilter = 0x7;

        public override string First(string input)
        {
            var parts = GetParts(input);
            UInt64 a = UInt64.Parse(parts[0]);
            UInt64 b = UInt64.Parse(parts[1]);
            var loops = int.Parse(parts[2]);
            return Judge(a, b, loops).ToString();
        }

        public override string Second(string input)
        {
            var parts = GetParts(input);
            UInt64 a = UInt64.Parse(parts[0]);
            UInt64 b = UInt64.Parse(parts[1]);
            var loops = int.Parse(parts[2]);
            return JudgeV2(a, b, loops).ToString();
        }

        public int Judge(UInt64 a, UInt64 b, int loops)
        {
            int judge = 0;
            for (int i = 0; i < loops; i++)
            {
                a = (a * Afactor) % 2147483647;
                b = (b * Bfactor) % 2147483647;
                if ((a & 0xFFFF) == (b & 0xFFFF)) judge++;
            }
            return judge;
        }


        public int JudgeV2(UInt64 a, UInt64 b, int loops)
        {
            int judge = 0;
            for (int i = 0; i < loops; i++)
            {
                a = GetNextValue(a, Afactor, Afilter);
                b = GetNextValue(b, Bfactor, Bfilter);
                if ((a&0xFFFF) == (b&0xFFFF)) judge++;
            }
            return judge;
        }

        public UInt64 GetNextValue(UInt64 start, UInt64 factor, UInt64 filter)
        {
            var val = start;
            do
            {
                val = (val * factor) % 2147483647;
            } while ((val & filter) != 0);
            return val;
        }
    }
}
