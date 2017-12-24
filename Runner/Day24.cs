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

            public override string ToString()
            {
                return string.Format("{0}/{1}", Link1, Link2);
            }
        }

        public class Bridge
        {
            public List<Component> Parts { get; set; }
            public bool Dirty { get; set; } = true;
            public int HashCode { get; set; }

            public Bridge(Component component)
            {
                Parts = new List<Component>();
                this.Add(component);
            }

            public Bridge(IEnumerable<Component> components)
            {
                Parts = new List<Component>(components);
                Dirty = true;
            }

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

            public void Add(Component cmp)
            {
                Parts.Add(cmp);
                Dirty = true;
            }

            public override int GetHashCode()
            {
                if (Dirty)
                {
                    UpdateState();
                }
                return HashCode;
            }

            private void UpdateState()
            {
                HashCode = Parts.Select(p => p.Link1 ^ p.Link2).Aggregate((p1, p2) => p1 ^ p2);
                //Parts = Parts.OrderBy(p => p.Link1).ThenBy(p => p.Link2).ToList();
                Dirty = false;
            }

            public override bool Equals(object obj)
            {
                var bridgeObj = obj as Bridge;
                if (bridgeObj == null) return false;
                if (bridgeObj.Parts.Count() != Parts.Count) return false;
                if (Dirty)
                {
                    UpdateState();
                }
                foreach (var part in Parts)
                {
                    if (!bridgeObj.Parts.Contains(part)) return false;
                }
                return true;
            }

            public int GetStrength()
            {
                return Parts.Sum(p => p.Link1) + Parts.Sum(p => p.Link2);
            }

            public override string ToString()
            {
                return string.Join("--", Parts.Select(p => p.ToString()).ToArray());
            }
        }

        public class Previous
        {
            Dictionary<int, List<Bridge>> _Before = new Dictionary<int, List<Bridge>>();

            public void AddBridge(Bridge bridge)
            {
                var key = bridge.GetHashCode();
                List<Bridge> list;
                if (!_Before.TryGetValue(key, out list))
                {
                    list = new List<Bridge>();
                    _Before[key] = list;
                }
                list.Add(bridge);
            }

            public bool SeenBefore(Bridge bridge)
            {
                var key = bridge.GetHashCode();
                List<Bridge> list;
                if (!_Before.TryGetValue(key, out list)) return false;
                foreach (var previous in list)
                {
                    if (previous.Equals(bridge)) return true;
                }
                return false;
            }

            public int MaxStrength()
            {
                return _Before.SelectMany(k => k.Value).Max(b => b.GetStrength());
            }

            public int BestBridgeStrength()
            {
                return  _Before.SelectMany(k => k.Value).GroupBy(b => b.Parts.Count()).OrderByDescending(g => g.Key).First()
                    .OrderByDescending(b => b.GetStrength()).First().GetStrength();
            }

            public override string ToString()
            {
                return string.Join(Environment.NewLine, 
                    _Before.SelectMany(k => k.Value)
                       .Select(b => b.ToString()).ToArray());
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
                queue.Enqueue(new Bridge(cmp));
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
                    var newBridge = new Bridge(bridge.Parts);
                    newBridge.Add(possible);
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
