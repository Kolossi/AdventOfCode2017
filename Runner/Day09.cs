using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    class Day09 : Day
    {
        public class Group
        {
            public int NestLevel { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            //public string Content { get; set; }
            public List<Group> SubGroups = new List<Group>();
            public int GarbageCount { get; set; }

            //public override string ToString()
            //{
            //    var sb = new StringBuilder();

            //    sb.AppendFormat("{0}'{1}'[{2}:{3}] subs:{4}, garbage:{5}",
            //        new string(' ',NestLevel), Content, StartIndex, EndIndex, SubGroups.Count, GarbageCount)
            //        .AppendLine();
            //    foreach (var sub in SubGroups)
            //    {
            //        sb.Append(sub.ToString());
            //    }
            //    return sb.ToString();
            //}
        }

        public override string First(string input)
        {
            return GetScore(input, g => g.NestLevel).ToString();
        }

        public override string FirstTest(string input)
        {
            if (input.StartsWith("#")) return GetScore(input.Substring(1),g=>g.NestLevel>0?1:0).ToString();
            return GetScore(input, g=>g.NestLevel).ToString();
        }

        private int GetScore(string input, Func<Group,int> scorer)
        {
            int score=0;
            var rootGroup = ProcessGroup(input, -1, new Group() { NestLevel = 0 });
            //Console.WriteLine(rootGroup.ToString());
            var toScore = new Queue<Group>();
            toScore.Enqueue(rootGroup);
            while (toScore.Count>0)
            {
                var group = toScore.Dequeue();
                score += scorer(group);
                foreach (var subGroup in group.SubGroups)
                {
                    toScore.Enqueue(subGroup);
                }
            }
            return score;
        }

        private Group ProcessGroup(string input, int index, Group group)
        {
            group.StartIndex = index;
            index++;
            char c;
            do
            {
                c = input[index];

                switch (c)
                {
                    case '{':
                        var subGroup = ProcessGroup(input, index, new Group() { NestLevel = group.NestLevel + 1 });
                        group.SubGroups.Add(subGroup);
                        index = subGroup.EndIndex + 1;
                        if (index < input.Length-1 && input[index] == ',') index++;
                        break;
                    case '}':
                        break;
                    case '<':
                        index = ProcessGarbage(group, input, index+1);
                        if (index < input.Length-1 && input[index] == ',') index++;
                        break;
                    case '!':
                        index += 2;
                        break;
                    default:
                        index++;
                        break;
                }
            }
            while (index < input.Length && input[index] != '}'/* && index < input.Length*/); ///let it blow...
            group.EndIndex = index;
            //group.Content = group.StartIndex == -1 ? string.Format("){0}(", input) : input.Substring(group.StartIndex, group.EndIndex - group.StartIndex + 1);
            index++;
            return group;
        }

        private int ProcessGarbage(Group group, string input, int index)
        {
            char c;
            do
            {
                c = input[index];
                switch (c)
                {
                    case '!':
                        index += 2;
                        break;
                    case '>':
                        break;
                    default:
                        group.GarbageCount++;
                        index++;
                        break;
                }
            }
            while (input[index] != '>');
            return index + 1;
        }

        public override string Second(string input)
        {
            return GetScore(input, g => g.GarbageCount).ToString();
        }
    }
}
