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
            public XYZ Pos0 { get; set; }
            public XYZ Vel0 { get; set; }
            public XYZ Accel0 { get; set; }
            public XYZ CurrentPos { get; set; }
            public XYZ CurrentVel { get; set; }

            public Particle PositionAt(int t)
            {
                if (t == 0)
                {
                    return new Particle() {CurrentPos = new XYZ(Pos0.X, Pos0.Y, Pos0.Z)};
                }

                return new Particle()
                {
                    CurrentPos = new XYZ(CoordAt(t, Pos0.X, Vel0.X, Accel0.X),
                        CoordAt(t, Pos0.Y, Vel0.Y, Accel0.Y),
                        CoordAt(t, Pos0.Z, Vel0.Z, Accel0.Z))
                };
            }

            public Particle Tick()
            {
                CurrentVel.X += Accel0.X;
                CurrentVel.Y += Accel0.Y;
                CurrentVel.Z += Accel0.Z;
                CurrentPos.X += CurrentVel.X;
                CurrentPos.Y += CurrentVel.Y;
                CurrentPos.Z += CurrentVel.Z;
                return this;
            }
        
            public long CoordAt(int t, long p, long v, long a)
            {
                // this doesn't actually work!, but good enough for part1
                decimal pd = p;
                decimal vd = v;
                decimal ad = a;
                decimal td = t;
                var res = decimal.Round(pd + vd * (td - 0.5m) + (ad * td * td) / 2.0m, MidpointRounding.AwayFromZero);

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
                    Pos0 = new XYZ(pos),
                    Vel0 = new XYZ(vel),
                    Accel0 = new XYZ(accel),
                    CurrentPos = new XYZ(pos),
                    CurrentVel = new XYZ(vel)
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
            var endPostions = particles.Select(p => new Particle() {Index = p.Index, CurrentPos = p.PositionAt(2000).CurrentPos});

            var distances = endPostions.Select(p => new {Index = p.Index, Dist = p.CurrentPos.GetDistance()})
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
                    //if (particle.Index == 0)
                    //{
                        //var posAtT = particle.PositionAt(t).CurrentPos;
                        //if (!posAtT.Equals(particle.CurrentPos))
                        //{
                        //    Console.WriteLine(string.Format("{2}@{3}s:calced {0}!=actual {1}", posAtT,
                        //        particle.CurrentPos, particle.Index, t));
                        //}
                    //}
                }
                var xyzGroups = particles.GroupBy(p => p.CurrentPos);
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
