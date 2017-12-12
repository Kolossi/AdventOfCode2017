using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day12 : Day
    {
        public override string First(string input)
        {
            var connections = GetConnections(input);
            return GetConnected(0, connections).Count().ToString();
        }

        private List<int> GetConnected(int target, Dictionary<int, List<int>> connections)
        {
            var connected = new List<int>();
            var toProcess = new Queue<int>();
            toProcess.Enqueue(target);
            connected.Add(target);

            while (toProcess.Any())
            {
                var newProgId = toProcess.Dequeue();
                var newProgs = connections[newProgId].Except(connected).ToArray();
                connected.AddRange(newProgs);
                foreach (var connectionId in newProgs)
                {
                    toProcess.Enqueue(connectionId);
                }
            }

            return connected;
        }

        private static Dictionary<int, List<int>> GetConnections(string input)
        {
            var connections = new Dictionary<int, List<int>>();
            var lines = GetLines(input);
            foreach (var line in lines)
            {
                var parts = line.Split(" <-> ");
                var progId = int.Parse(parts[0]);
                connections[progId] = new List<int>(parts[1].Split(", ").Select(w => int.Parse(w)));
            }

            return connections;
        }

        public override string Second(string input)
        {
            var connections = GetConnections(input);
            return GetGroups(connections).ToString();
        }

        private int GetGroups(Dictionary<int, List<int>> connections)
        {
            var progGroup = new Dictionary<int, int>();
            var toProcess = new Queue<int>();

            while (progGroup.Keys.Count() < connections.Keys.Count())
            {
                var target = connections.Keys.Except(progGroup.Keys).First();
                var connected = GetConnected(target, connections);
                foreach (var connection in connected)
                {
                    progGroup[connection] = target;
                }
            }
            return progGroup.Values.Distinct().Count();
        }
    }
}
