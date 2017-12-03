using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    class Day03 :  Day
    {
        public class XY : IEquatable<XY>
        {
            public int X { get; set; }
            public int Y { get; set; }
            public XY(int x, int y)
            {
                X = x;
                Y = y;
            }
            public override string ToString()
            {
                return string.Format("[{0},{1}]",X,Y);
            }

            public override bool Equals(object obj)
            {
                if (obj as XY == null) return base.Equals(obj);
                XY objXY = (XY)obj;
                return Equals(objXY);
            }

            public bool Equals(XY objXY)
            {
                return (objXY.X == X && objXY.Y == Y);
            }
        }

        private XY[] Directions = new XY[] {
                new XY(0, 1),
                new XY(1, 0),
                new XY(0, -1),
                new XY(-1, 0)
            };

        private XY Position;

        private int Direction;

        private Dictionary<XY, int> SumMap;

        private int TargetSum;

        private XY TargetXY;

        public override string First(string input)
        {
            Position = new XY(0, 0);
            Direction = 1;
            return DoWalk(input, Walk);
        }

        public override string Second(string input)
        {
            SumMap = new Dictionary<XY, int>();
            SumMap[new XY(0, 0)] = 1;
            TargetSum = int.Parse(input);
            Position = new XY(0, 0);
            Direction = 1;

            DoWalk(int.MaxValue.ToString(), PopulateWalk);

            return SumMap[TargetXY].ToString();
        }

        private string PositionDistance()
        {
            return (Math.Abs(Position.X) + Math.Abs(Position.Y)).ToString();
        }

        private string DoWalk(string input, Func<int,int,int> walkFunc)
        {
            var toGo = int.Parse(input)-1;
            int legSize = 1;
            while (toGo > 0)
            {
                toGo = walkFunc(toGo, legSize);
                toGo = walkFunc(toGo, legSize);
                legSize++;
            }

            var dist = PositionDistance();
            return dist;
        }

        private int Walk(int toGo, int legSize)
        {
            int dist = Math.Min(toGo, legSize);
            Position.X += Directions[Direction].X * dist;
            Position.Y += Directions[Direction].Y * dist;
            Direction = (Direction-1+4) % 4;
            return toGo - dist;
        }

        private int PopulateWalk(int toGo, int legSize)
        {
            int dist = Math.Min(toGo, legSize);
            for (int i = 0; i < dist; i++)
            {
                Position.X += Directions[Direction].X;
                Position.Y += Directions[Direction].Y;
                var sum = GetSum(Position);
                Console.WriteLine(string.Format("{0}={1}", Position, sum));
                if (sum>TargetSum)
                {
                    TargetXY = Position;
                    return 0;
                }
            }
            Direction = (Direction - 1 + 4) % 4;
            return toGo;
        }

        private int GetSum(XY position)
        {
            int sum = 0;
            for (int x = -1; x <= 1 ; x++)
            {
                for (int y = -1; y <= 1 ; y++)
                {
                    if (x == 0 && y == 0) continue;
                    XY key = new XY(position.X + x, position.Y + y);
                    if (!SumMap.ContainsKey(key)) continue;
                    sum += SumMap[key];
                }
            }
            SumMap[position] = sum;

            return sum;
        }
    }
}
