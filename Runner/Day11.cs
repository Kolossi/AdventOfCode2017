using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    class Day11 : Day
    {
        Dictionary<string, Func<XY, XY>> Offsets = new Dictionary<string, Func<XY, XY>>() {
            { "n", xy=>new XY(xy.X, xy.Y+1) },
            { "ne",xy=> new XY(xy.X+1, xy.Y+(xy.X % 2 == 0 ? 1 : 0)) },
            { "se",xy=> new XY(xy.X+1, xy.Y-(xy.X % 2 == 0 ? 0 : 1)) },
            { "s", xy=>new XY(xy.X, xy.Y-1) },
            { "sw", xy=>new XY(xy.X-1,xy.Y-(xy.X % 2 == 0 ? 0 : 1) ) },
            { "nw", xy=>new XY(xy.X-1, xy.Y+(xy.X % 2 == 0 ? 1 : 0)) },
        };

        public override string First(string input)
        {
            var dirs = input.Split(",");
            var xy = new XY(0, 0);
            foreach (var dir in dirs)
            {
                xy = Offsets[dir](xy);
            }

            return GetDistance(xy).ToString();
        }

        private int GetDistance(XY xy)
        {
            xy = new XY(xy.X, xy.Y);
            int dist = 0;
            while (xy.X!=0)
            {
                string dir = xy.X > 0 ? (xy.Y <= 0 ? "nw" : "sw") : (xy.Y <= 0 ? "ne" : "se");
                xy = Offsets[dir](xy);
                dist++;
            }
            dist += Math.Abs(xy.Y);
            return dist;
        }

        public override string Second(string input)
        {
            var dirs = input.Split(",");
            var xy = new XY(0, 0);
            int maxDistance = 0;
            foreach (var dir in dirs)
            {
                xy = Offsets[dir](xy);
                maxDistance = Math.Max(maxDistance, GetDistance(xy));
            }

            return maxDistance.ToString();
        }
    }
}
