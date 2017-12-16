using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day16 : Day
    {
        // result of unnecessary optimisation!:
        public static char[] Buffer;
        public static int Length;

        public override string First(string input)
        {
            var parts = input.Split('|');
            var programs = parts[0].ToArray();
            Length = programs.Length;
            Buffer = new char[Length];
            var instructions = GetInstructions(parts[1]);
            return string.Join("",Process(programs, instructions));
        }

        public char[] Process(char[] programs, List<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                instruction.Apply(programs);
            }
            return programs;
        }

        public abstract class Instruction
        {
            public abstract void Apply(char[] programs);
        }

        public class Spin : Instruction
        {
            public int SliceLength;
            public override void Apply(char[] programs)
            {
                Array.Copy(programs, Buffer, Length);
                Array.Copy(Buffer, Length - SliceLength, programs, 0, SliceLength);
                Array.Copy(Buffer, 0, programs, SliceLength, Length - SliceLength);
            }
        }

        public class Exchange : Instruction
        {
            public int Index1;
            public int Index2;
            private char Tmp;
            public override void Apply(char[] programs)
            {
                Tmp = programs[Index1];
                programs[Index1] = programs[Index2];
                programs[Index2] = Tmp;
            }
        }

        public class Swap : Instruction
        {
            public char Char1;
            public char Char2;
            private int Index1;
            private int Index2;
            private char Tmp;
            private bool Found1;
            private bool Found2;
            public override void Apply(char[] programs)
            {
                Found1 = false;
                Found2 = false;
                for (int i = 0; i < Length; i++)
                {
                    Tmp = programs[i];
                    if (Tmp==Char1)
                    {
                        Index1 = i;
                        Found1 = true;
                    }
                    if (Tmp == Char2)
                    {
                        Index2 = i;
                        Found2 = true;
                    }
                    if (Found1 && Found2) break;
                }
                programs[Index1] = Char2;
                programs[Index2] = Char1;
            }
        }

        public List<Instruction> GetInstructions(string input)
        {
            var instructions = new List<Instruction>();
            var instructionTexts = input.Split(",");
            foreach (var instructionText in instructionTexts)
            {
                switch (instructionText[0])
                {
                    case 's':
                        var sliceLength = int.Parse(instructionText.Substring(1));
                        instructions.Add(new Spin() { SliceLength = sliceLength });
                        break;
                    case 'x':
                        var parts = instructionText.Substring(1).Split('/');
                        var p1 = int.Parse(parts[0]);
                        var p2 = int.Parse(parts[1]);
                        instructions.Add(new Exchange() { Index1 = p1, Index2 = p2 });
                        break;
                    case 'p':
                        parts = instructionText.Substring(1).Split('/');
                        var c1 = parts[0][0];
                        var c2 = parts[1][0];
                        instructions.Add(new Swap() { Char1 = c1, Char2 = c2 });
                        break;
                    default:
                        break;
                }
            }
            return instructions;
        }

        public override string SecondTest(string input)
        {
            var parts = input.Split('|');
            var programs = parts[0].ToArray();
            Length = programs.Length;
            Buffer = new char[Length];
            var instructions = GetInstructions(parts[1]);
            programs = Process(programs, instructions);
            programs = Process(programs, instructions);
            return string.Join("", programs);
        }

        public int FindRepeatIndex(char[] programs, List<Instruction> instructions)
        {
            var originalPrograms = new char[Length];
            Array.Copy(programs, originalPrograms, Length);
            bool match;

            for (int i = 0; i < 1000000000; i++)
            {
                programs = Process(programs, instructions);
                match = true;
                for (int j = 0; j < Length; j++)
                {
                    if (programs[j] != originalPrograms[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i+1;
                }
            }
            throw new InvalidOperationException();
        }

        public override string Second(string input)
        {
            var parts = input.Split('|');
            var originalPrograms = parts[0].ToArray();
            var programs = parts[0].ToArray();
            Length = programs.Length;
            Buffer = new char[Length];
            var instructions = GetInstructions(parts[1]);
            var repeatIndex = FindRepeatIndex(programs, instructions);
            var toRun = 1000000000 % repeatIndex;
            for (int i = 0; i < toRun; i++)
            {
                Process(programs, instructions);

            }
            return string.Join("", programs);
        }
    }
}
