using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day19 : Day
    {
        private XY[] Directions = new XY[] {
                new XY(0, 1),
                new XY(1, 0),
                new XY(0, -1),
                new XY(-1, 0)
            };

        class WalkResult
        {
            public string Message { get; set; }
            public int Steps { get; set; }
        }

        private static char[,] GetMap(string input)
        {
            var lines = GetLines(input);
            var map = new char[lines[0].Length, lines.Length];
            var y = 0;
            foreach (var line in lines)
            {
                for (int x = 0; x < line.Length; x++)
                {
                    map[x, y] = line[x];
                }
                y++;
            }
            return map;
        }

        private WalkResult WalkMap(char[,] map, XY pos, XY dir)
        {
            var message = new StringBuilder();
            int steps = 1;
            bool finished = false;
            do
            {
                var mapChar = map[pos.X, pos.Y];
                switch (mapChar)
                {
                    case '+':
                        dir = NewDir(map, pos, dir);
                        break;
                    case '-':
                    case '|':
                        break;
                    case ' ':
                        finished = true;
                        steps--;
                        break;
                    default:
                        message.Append(mapChar);
                        break;
                }
                pos.X += dir.X;
                pos.Y += dir.Y;
                steps++;
            } while (!finished && pos.X >= 0 && pos.Y >= 0 && pos.X < map.GetLength(0) && pos.Y < map.GetLength(1));
            steps--;
            return new WalkResult()
            {
                Message = message.ToString(),
                Steps = steps
            };
        }

        private XY NewDir(char[,] map, XY pos, XY dir)
        {
            foreach (var newDir in Directions.Where(d => d != dir))
            {
                //reverse
                if (newDir.X == -dir.X && newDir.Y == -dir.Y) continue;

                var newPos = new XY(pos.X + newDir.X, pos.Y + newDir.Y);
                if (newPos.X < 0 || newPos.Y < 0 || newPos.X >= map.GetLength(0) || newPos.Y >= map.GetLength(1)) continue;

                var mapChar = map[newPos.X,newPos.Y];

                if (mapChar != ' ' && mapChar != '+') return newDir;
            }
            throw new InvalidOperationException("Lost");
        }

        public override string First(string input)
        {
            string[] lines = GetLines(input);
            char[,] map = GetMap(input);
            var pos = new XY(lines[0].IndexOf("|"), 0);
            var dir = Directions[0];
            return WalkMap(map, pos, dir).Message;
        }

        public override string Second(string input)
        {
            string[] lines = GetLines(input);
            char[,] map = GetMap(input);
            var pos = new XY(lines[0].IndexOf("|"), 0);
            var dir = Directions[0];
            return WalkMap(map, pos, dir).Steps.ToString();
        }
    }
}
