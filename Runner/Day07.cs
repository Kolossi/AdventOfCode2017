using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day07 :  Day
    {

        public class Program
        {
            public string Name { get; set; }
            public int Weight { get; set; }
            public int TowerWeight { get; set; }
            public Program Base { get; set; }
            public List<Program> Children { get; set; }
            public Program()
            {
                Children = new List<Program>();
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(Name).Append(":[");
                sb.Append(string.Join(",", Children.Select(c => c.TowerWeight.ToString()).ToArray()));
                sb.Append("](");
                sb.Append(TowerWeight).Append(")");
                return sb.ToString();
            }
        }

        public override string First(string input)
        {
            return FindBase(GetLines(input));
        }

        public string FindBase(string[] lines)
        {
            var baseLines = lines.Where(l => l.Contains("->"));
            var names = baseLines.Select(l=>GetParts(l)[0]);
            var notBaseNames = baseLines.Select(l => l.Split(" -> ")[1]).SelectMany(s => s.Split(", "));
            return names.Except(notBaseNames).First();
        }

        public override string Second(string input)
        {
            var lines = GetLines(input);
            var programs=MakePrograms(lines);
            AssignBase(lines,programs);
            var unbalancedProg = FindUnbalancedProgram(programs);
            return GetFix(unbalancedProg);
        }

        public List<Program> MakePrograms(string[] lines)
        {
            var programs = new List<Program>();
            foreach (var line in lines)
            {
                var parts = GetParts(line);
                var name = parts[0];
                var weight = int.Parse(parts[1].Substring(1, parts[1].Length - 2));
                programs.Add(new Program() { Name = name, Weight = weight });
            }

            return programs;
        }

        public void AssignBase(string[] lines,List<Program> programs)
        {
            foreach (var line in lines.Where(l=>l.Contains("->")))
            {
                var baseName = GetParts(line)[0];
                var baseProg = programs.Find(p => p.Name == baseName);
                var names = line.Split(" -> ")[1].Split(", ");
                var programsWithBase = names.Select(n => programs.Find(p => p.Name == n));
                foreach (var prog in programsWithBase)
                {
                    prog.Base = baseProg;
                    baseProg.Children.Add(prog);
                }
            }
        }

        public Program FindUnbalancedProgram(List<Program> programs)
        {
            var toProcess = new Stack<Program>();
            var firstBase = programs.First(p => p.Base == null);
            var toWalk = new Queue<Program>();
            toWalk.Enqueue(firstBase);

            while (toWalk.Any())
            {
                var prog = toWalk.Dequeue();
                toProcess.Push(prog);
                foreach (var child in prog.Children)
                {
                    toWalk.Enqueue(child);
                }
            }
           
            while (toProcess.Any())
            {
                var prog = toProcess.Pop();
                if (IsUnbalanced(prog)) return prog;
                prog.TowerWeight = prog.Weight + prog.Children.Sum(p => p.TowerWeight);
            }
            throw new InvalidOperationException("NOT FOUND");
        }

        private bool IsUnbalanced(Program prog)
        {
            var childWeights = prog.Children.Select(p => p.TowerWeight);
            if (childWeights.Any())
            {
                var firstWeight = childWeights.ToArray()[0];
                if (childWeights.Any(w => w != firstWeight))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetFix(Program program)
        {
            var commonTowerWeight = program.Children.GroupBy(c => c.TowerWeight).OrderByDescending(g => g.Count()).First().First().TowerWeight;
            var wrongWeightProgram = program.Children.First(c => c.TowerWeight != commonTowerWeight);
            var correction = wrongWeightProgram.Weight - (wrongWeightProgram.TowerWeight - commonTowerWeight);
            return correction.ToString();
        }
    }
}
