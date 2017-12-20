using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day20 : Day
    {
        public class XYZ : IEquatable<XYZ>
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public XYZ(string input)
            {
                var parts = input.Split(",");
                X = int.Parse(parts[0].Trim());
                Y = int.Parse(parts[1].Trim());
                Z = int.Parse(parts[2].Trim());
            }
            public XYZ(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public override string ToString()
            {
                return string.Format("[{0},{1},{2}]", X, Y, Z);
            }

            public override bool Equals(object obj)
            {
                if (obj as XYZ == null) return base.Equals(obj);
                XYZ objXYZ = (XYZ) obj;
                return Equals(objXYZ);
            }

            public bool Equals(XYZ objXYZ)
            {
                return (objXYZ.X == X && objXYZ.Y == Y && objXYZ.Z == Z);
            }

            public override int GetHashCode()
            {
                return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
            }

            public long GetDistance()
            {
                return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
            }
        }


        public class Particle
        {
            public int Index { get; set; }
            public XYZ Pos { get; set; }
            public XYZ Vel { get; set; }
            public XYZ Accel { get; set; }

            public XYZ PositionAt(int t)
            {
                return new XYZ(CoordAt(t, Pos.X, Vel.X, Accel.X),
                    CoordAt(t, Pos.Y, Vel.Y, Accel.Y),
                    CoordAt(t, Pos.Z, Vel.Z, Accel.Z));
            }
        
            public int CoordAt(int t, int p, int v, int a)
            {
                //var res = (p + (t * (v + (t) * a)));
                float pf = p;
                float vf = v;
                float af = a;
                float tf = t-0.01f;
                var res = (pf + vf*tf + (af*tf*tf)/2.0f);
                //Console.WriteLine(res);

                return (int)res;
            }
        }

        public List<Particle> GetParticles(string input)
        {
            var particles = new List<Particle>();
            var lines = GetLines(input);
            int i = 0;
            foreach (var line in lines)
            {
                var line2 = line.Replace("<", "").Replace(">,", "").Replace(">", "").Replace(" ","");
                var parts = line2.Split("a=");
                var accel = parts[1];
                parts = parts[0].Split("v=");
                var vel = parts[1];
                var pos = parts[0].Split("p=")[1];
                var particle = new Particle()
                {
                    Index = i++,
                    Pos = new XYZ(pos),
                    Vel = new XYZ(vel),
                    Accel = new XYZ(accel)
                };

                particles.Add(particle);

            }
            return particles;
        }
        public override string First(string input)
        {
            //126 too high
            //122 too low!
            // zero-based, 125
            var particles = GetParticles(input);
            var sb = new StringBuilder();
            sb.AppendLine();


            for (int t = 0; t < 10000; t++)
            {
                var endPostions = particles.Select(p => new Particle() { Index = p.Index, Pos = p.PositionAt(t) });

                var distances = endPostions.Select(p => new { Index = p.Index, Dist = p.Pos.GetDistance() })
                    .OrderBy(d => d.Dist);
                var nearest = distances.First().Index;
                Console.WriteLine(nearest);
            }
            return "";
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
