using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    class Day22 : Day
    {
        public class XY : IEquatable<XY>
        {
            public long X { get; set; }
            public long Y { get; set; }
            public XY(long x, long y)
            {
                X = x;
                Y = y;
            }
            public override string ToString()
            {
                return string.Format("[{0},{1}]", X, Y);
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

            public override int GetHashCode()
            {
                return X.GetHashCode() ^ Y.GetHashCode();
            }
        }

        private XY[] Directions = new XY[] {
                new XY(0, -1),
                new XY(1, 0),
                new XY(0, 1),
                new XY(-1, 0)
            };

        public enum State
        {
            Clean = 0,
            Weakened = 1,
            Infected = 2,
            Flagged = 3
        }

        Dictionary<long, Dictionary<long,State>> NodeState;

        int DirectionIndex = 0;

        XY Position;

        long InfectedCount { get; set; }
 
        public void Burst()
        {
            bool infected = IsNotClean(Position.X, Position.Y);
            if (infected)
            {
                DirectionIndex = (DirectionIndex + 1) % 4;
                Clean(Position.X, Position.Y);
            }
            else
            {
                DirectionIndex = (DirectionIndex + 4 - 1) % 4;
                SetState(Position.X, Position.Y, State.Infected);
            }

            var direction = Directions[DirectionIndex];
            Position.X += direction.X;
            Position.Y += direction.Y;
        }


        public void BurstV2()
        {
            var state = GetState(Position.X, Position.Y);
            switch (state)
            {
                case State.Clean:
                    DirectionIndex = (DirectionIndex + 4 - 1) % 4;
                    break;
                case State.Infected:
                    DirectionIndex = (DirectionIndex + 1) % 4;
                    break;
                case State.Flagged:
                    DirectionIndex = (DirectionIndex + 2) % 4;
                    break;
                case State.Weakened:
                default:
                    break;
            }

            var newState = (State)((((int)state) + 1) % 4);

            SetState(Position.X, Position.Y, newState);

            var direction = Directions[DirectionIndex];
            Position.X += direction.X;
            Position.Y += direction.Y;
        }

        private State GetState(long x, long y)
        {
            Dictionary<long, State> nodeStateX;
            if (!NodeState.TryGetValue(y, out nodeStateX))
            {
                return State.Clean;
            }
            State state;
            if (!nodeStateX.TryGetValue(x, out state))
            {
                return State.Clean;
            }
            return state;
        }

        private bool IsNotClean(long x, long y)
        {
            return GetState(x, y) != State.Clean;
        }

        private void Clean(long x, long y)
        {
            Dictionary<long, State> nodeStateX;
            if (!NodeState.TryGetValue(y, out nodeStateX))
            {
                return;
            }
            if (nodeStateX.ContainsKey(x)) nodeStateX.Remove(x);
        }

        private void SetState(long x,long y,State newState)
        {
            if (newState==State.Clean)
            {
                Clean(x,y);
                return;
            }

            Dictionary<long, State> nodeStateX;

            if (!NodeState.TryGetValue(y, out nodeStateX))
            {
                nodeStateX = new Dictionary<long, State>();
                NodeState[y] = nodeStateX;
            }
            nodeStateX[x]=newState;
            if (newState==State.Infected) InfectedCount++;
        }

        private void GetInitialState(string[] lines)
        {
            NodeState = new Dictionary<long, Dictionary<long,State>>();
            for (long y = 0; y < lines.Length - 1; y++)
            {
                for (long x = 0; x < lines[0].Length; x++)
                {
                    var c = lines[y][(int)x];
                    if (c == '.') continue;
                    SetState(x, y,State.Infected);
                }

            }
            InfectedCount = 0;
            var startCoord = (long)(lines[1].Length / 2);
            Position = new XY(startCoord, startCoord);
            DirectionIndex = 0;
        }

        public string TestStart = "..#\r\n#..\r\n...";

        public override string FirstTest(string input)
        {
            return First(string.Format("{0}\r\n{1}", TestStart, input));
        }

        public override string SecondTest(string input)
        {
            return Second(string.Format("{0}\r\n{1}", TestStart, input));
        }

        public override string First(string input)
        {
            var lines = GetLines(input);
            var bursts = int.Parse(lines[lines.Length - 1]);
            GetInitialState(lines);
            for (int burst = 0; burst < bursts; burst++)
            {
                Burst();
            }
            return InfectedCount.ToString();
        }

        public override string Second(string input)
        {
            var lines = GetLines(input);
            var bursts = int.Parse(lines[lines.Length - 1]);
            GetInitialState(lines);
            for (int burst = 0; burst < bursts; burst++)
            {
                BurstV2();
            }
            return InfectedCount.ToString();

        }
    }
}
