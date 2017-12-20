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
            public long X { get; set; }
            public long Y { get; set; }
            public long Z { get; set; }

            public XYZ(string input)
            {
                var parts = input.Split(",");
                X = long.Parse(parts[0].Trim());
                Y = long.Parse(parts[1].Trim());
                Z = long.Parse(parts[2].Trim());
            }
            public XYZ(long x, long y, long z)
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
                XYZ objXYZ = obj as XYZ;
                if (objXYZ == null) return false;
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
            public XYZ Current { get; set; }

            public Particle PositionAt(int t)
            {
                if (t == 0)
                {
                    Current = new XYZ(Pos.X, Pos.Y, Pos.Z);
                }
                else
                {


                    Current = new XYZ(CoordAt(t, Pos.X, Vel.X, Accel.X),
                        CoordAt(t, Pos.Y, Vel.Y, Accel.Y),
                        CoordAt(t, Pos.Z, Vel.Z, Accel.Z));
                }
                return this;
            }

            public Particle Tick()
            {
                Vel.X += Accel.X;
                Vel.Y += Accel.Y;
                Vel.Z += Accel.Z;
                Pos.X += Vel.X;
                Pos.Y += Vel.Y;
                Pos.Z += Vel.Z;
                return this;
            }
        
            public long CoordAt(int t, long p, long v, long a)
            {
                // this doesn't actually work!, but good enough for part1
                decimal pd = p;
                decimal vd = v;
                decimal ad = a;
                decimal td = t;
                var res = decimal.Round(pd + vd*td + (ad*((td+0.5m)* (td + 0.5m)) /2.0m),MidpointRounding.AwayFromZero);

                return (long)res;
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
            var particles = GetParticles(input);
            var sb = new StringBuilder();
            sb.AppendLine();

            Particle nearest = null;
            var endPostions = particles.Select(p => new Particle() {Index = p.Index, Pos = p.PositionAt(2000).Current});

            var distances = endPostions.Select(p => new {Index = p.Index, Dist = p.Pos.GetDistance()})
                .OrderBy(d => d.Dist);
            nearest = particles.First(p => p.Index == distances.First().Index);

            return nearest.Index.ToString();
        }

        public override string Second(string input)
        {
            var particles = GetParticles(input);

            for (int t = 0; t < 1000; t++)
            {
                foreach (var particle in particles)
                {
                    particle.Tick();
                }
                var xyzGroups = particles.GroupBy(p => p.Pos);
                var collisionParticles = xyzGroups.Where(g => g.Count() > 1)
                    .SelectMany(g => g).Select(p => p.Index);

                if (collisionParticles.Any())
                {
                    particles = particles.Where(p => !collisionParticles.Contains(p.Index)).ToList();
                }

                if (particles.Count == 1) break;
            }
            return particles.Count.ToString();
        }
    }
}
