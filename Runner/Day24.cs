using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day24 : Day
    {
        public class Component
        {
            public int Link1 { get; set; }
            public int Link2 { get; set; }
        }

        public class Bridge
        {
            public List<Component> Parts { get; set; }

            public int GetNextLink()
            {
                if (Parts.Count == 1)
                {
                    var cmp = Parts[0];
                    return cmp.Link1 == 0 ? cmp.Link2 : cmp.Link1;
                }
                var lstCmp = Parts[Parts.Count() - 1];
                var beforeCmp = Parts[Parts.Count() - 2];
                return beforeCmp.Link1 == lstCmp.Link1 || beforeCmp.Link2 == lstCmp.Link1 ? lstCmp.Link2 : lstCmp.Link1;
            }

            public int GetStrength()
            {
                return Parts.Sum(p => p.Link1) + Parts.Sum(p => p.Link2);
            }
        }

        public class Previous
        {
            List<Bridge> _Before = new List<Bridge>();

            public void AddBridge(Bridge bridge)
            {
                    _Before.Add(bridge);
            }

            public int MaxStrength()
            {
                return _Before.Max(b => b.GetStrength());
            }

            public int BestBridgeStrength()
            {
                return  _Before.GroupBy(b => b.Parts.Count()).OrderByDescending(g => g.Key).First()
                    .OrderByDescending(b => b.GetStrength()).First().GetStrength();
            }

            public override string ToString()
            {
                return string.Join(Environment.NewLine, 
                    _Before.Select(b => b.ToString()).ToArray());
            }
        }

        public class Inventory
        {
            public Dictionary<int,List<Component>> LinkLookup { get; set; }
            public HashSet<Component> All { get; set; }

            public Inventory()
            {
                LinkLookup = new Dictionary<int, List<Component>>();
                All = new HashSet<Component>();
            }
            public void AddCmp(Component cmp)
            {
                AddCmp(cmp, cmp.Link1);
                if (cmp.Link1 != cmp.Link2) AddCmp(cmp, cmp.Link2);
            }

            public void AddCmp(Component cmp, int key)
            {
                List<Component> list;
                if (!LinkLookup.TryGetValue(key, out list))
                {
                    list = new List<Component>();
                    LinkLookup[key] = list;
                }
                list.Add(cmp);
                All.Add(cmp);
            }
        }

        public static void AddCmp(Dictionary<int, List<Component>> components, int key, Component value)
        {
            List<Component> list;
            if (!components.TryGetValue(key,out list))
            {
                list = new List<Component>();
                components[key]=list;
            }
            list.Add(value);
        }

        private static Inventory GetComponents(string input)
        {
            var inventory = new Inventory();
            var lines = GetLines(input);
            var components = new Dictionary<int, List<Component>>();
            foreach (var line in lines)
            {
                var parts = line.Split("/");
                var cmp = new Component()
                {
                    Link1 = int.Parse(parts[0]),
                    Link2 = int.Parse(parts[1])
                };
                inventory.AddCmp(cmp);
            }
            return inventory;
        }
        
        private static Previous ProcessBridges(Inventory inventory)
        {
            var queue = new Queue<Bridge>();
            var previous = new Previous();
            foreach (var cmp in inventory.LinkLookup[0])
            {
                var bridge = new Bridge() { Parts = new List<Component>() };
                bridge.Parts.Add(cmp);
                queue.Enqueue(bridge);
            }
            while (queue.Any())
            {
                var bridge = queue.Dequeue();
                previous.AddBridge(bridge);
                var nextLink = bridge.GetNextLink();
                var possibles = inventory.LinkLookup.GetValueOrDefault(nextLink);
                if (possibles == null) continue;
                foreach (var possible in possibles)
                {
                    if (bridge.Parts.Contains(possible)) continue;
                    var newBridge = new Bridge() { Parts = new List<Component>(bridge.Parts) };
                    newBridge.Parts.Add(possible);
                    queue.Enqueue(newBridge);
                }
            }

            return previous;
        }

        public override string First(string input)
        {
            var inventory = GetComponents(input);
            Previous previous = ProcessBridges(inventory);
            return previous.MaxStrength().ToString();
        }

        public override string Second(string input)
        {
            var inventory = GetComponents(input);
            Previous previous = ProcessBridges(inventory);
            return previous.BestBridgeStrength().ToString();
        }
    }
}
