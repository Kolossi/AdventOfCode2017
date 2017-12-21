using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day21 : Day
    {
        private const string StartPattern = ".#./..#/###";

        public static bool[,] GetArray(string pattern)
        {
            var size = pattern.IndexOf("/");
            var array = new bool[size, size];
            int y = 0;
            foreach (var line in pattern.Split("/"))
            {
                for (int x = 0; x < size; x++)
                {
                    array[x, y] = line[x] == '#' ? true : false;
                }
                y++;
            }
            return array;
        }

        public static bool[,] RotateArray(bool[,] array)
        {
            var size = array.GetLength(0);
            var result = new bool[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    //transpose and reverse resultant rows = rotate
                    result[x, y] = array[y, size - x - 1];
                }
            }
            return result;
        }
        
        public static bool[,] FlipArray(bool[,] array)
        {
            var size = array.GetLength(0);
            var result = new bool[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    result[x, y] = array[x, size - y - 1];
                }
            }
            return result;
        }

        public string ToString(bool[,] array)
        {
            var size = array.GetLength(0);
            var sb = new StringBuilder();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    sb.Append(array[x, y] ? '#' : '.');
                }
                sb.AppendLine();
            }
            return sb.ToString();

        }
        public class Rule
        {
            public bool[,] From { get; set; }
            public bool[,] To { get; set; }

            public bool IsMatch(bool[,] array, int xOffset, int yOffset, int size)
            {
                if (size != From.GetLength(0)) return false;
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        if (From[x, y] != array[x+xOffset, y+yOffset]) return false;
                    }
                }
                return true;
            }
        }

        private List<Rule> Rules;

        public int CountArray(bool[,] array)
        {
            int count = 0;
            var size = array.GetLength(0);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (array[x, y]) count++;
                }
            }
            return count;
        }

        private void GetRules(Queue<string> lines)
        {
            Rules = new List<Rule>();
            while (lines.Any())
            {
                var line = lines.Dequeue();
                var parts = line.Split(" => ");
                var to = GetArray(parts[1]);
                var from = GetArray(parts[0]);

                for (int f = 0; f < 2; f++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Rules.Add(new Rule()
                        {
                            From = from,
                            To = to
                        });

                        if (i != 3) from = RotateArray(from);
                    }
                    if (f == 0) from = FlipArray(from);
                }
            }
        }
        
        public class Section
        {
            public bool[,] Data { get; set; }
            public int sectionX { get; set; }
            public int sectionY { get; set; }
        }

        private bool[,] Enhance(bool[,] array)
        {
            var size = array.GetLength(0);
            if (size < 4)
            {
                return EnhanceFromRule(array, 0, 0, size);
            }
            var sectionSize = size % 2 == 0 ? 2 : 3;
            var sections = new List<Section>();
            for (int y = 0; y < size; y += sectionSize)
            {
                for (int x = 0; x < size; x += sectionSize)
                {
                    sections.Add(new Section()
                    {
                        Data =EnhanceFromRule(array,x,y,sectionSize),
                        sectionX = x/sectionSize,
                        sectionY = y/sectionSize
                    });
                }
            }
            var newSectionSize = sections[0].Data.GetLength(0);
            var newSize = newSectionSize * (size / sectionSize);
            var result = new bool[newSize, newSize];
            foreach (var section in sections)
            {
                CopyTo(result, section.sectionX * newSectionSize, section.sectionY * newSectionSize, section.Data);
            }
            return result;
        }

        private void CopyTo(bool[,] dest, int destX, int destY, bool[,] source)
        {
            var size = source.GetLength(0);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    dest[destX + x, destY + y] = source[x, y];
                }
            }
        }

        private bool[,] EnhanceFromRule(bool[,] array, int xOffset, int yOffset, int size)
        {
            foreach (var rule in Rules)
            {
                if (rule.IsMatch(array, xOffset, yOffset, size)) return rule.To;
            }
            throw new InvalidOperationException();
        }

        public override string First(string input)
        {
            var lines = new Queue<string>(GetLines(input));
            var iterations = int.Parse(lines.Dequeue());
            GetRules(lines);
            var array = GetArray(StartPattern);
            for (int i = 0; i < iterations; i++)
            {
                array = Enhance(array);
                //Console.WriteLine();
                //Console.WriteLine(ToString(array));
            }

            return CountArray(array).ToString();
        }


        public override string Second(string input)
        {
            return First(input);
        }
    }
}
