using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day08 :  Day
    {
        public Dictionary<string, int> Registers;

        public Dictionary<string, Func<int, int, bool>> condFuncs = new Dictionary<string, Func<int, int, bool>>()
        {
            { ">", (a,b) => a > b },
            { ">=", (a,b) => a >= b },
            { "<", (a,b) => a < b },
            { "<=", (a,b) => a <= b },
            { "==", (a,b) => a == b },
            { "!=", (a,b) => a != b }
        };

        public override string First(string input)
        {
            var lines = GetLines(input);
            ProcessInstructions(lines);
            return Registers.Values.Max().ToString();
        }

        public int GetVal(string key)
        {
            if (!Registers.ContainsKey(key)) Registers[key] = 0;
            return Registers[key];
        }

        public override string Second(string input)
        {
            var lines = GetLines(input);
            int maxVal = ProcessInstructions(lines);
            return maxVal.ToString();
        }

        private int ProcessInstructions(string[] lines)
        {
            Registers = new Dictionary<string, int>();
            int maxVal = 0;

            foreach (var line in lines)
            {
                var parts = GetParts(line);
                var reg = parts[0];
                var inc = (parts[1] == "dec" ? -1 : 1) * int.Parse(parts[2]);
                var condReg = parts[4];
                var cond = parts[5];
                var condVal = int.Parse(parts[6]);


                var newVal = GetVal(reg) + (condFuncs[cond](GetVal(condReg), condVal) ? inc : 0);
                maxVal = Math.Max(maxVal, newVal);
                Registers[reg] = newVal;
            }

            return maxVal;
        }
    }
}
