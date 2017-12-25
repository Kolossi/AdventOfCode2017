using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day25 : Day
    {
        class StateTransition
        {
            public char State { get; set; }
            public long Offset0 { get; set; }
            public bool Value0 { get; set; }
            public char NewState0 { get; set; }
            public long Offset1 { get; set; }
            public bool Value1 { get; set; }
            public char NewState1 { get; set; }
        }

        public override string First(string input)
        {
            var tape = new Dictionary<long, bool>();
            var lines = GetLines(input).Select(l => l.Replace(".", "").Replace(":", "")).ToArray();
            char state = GetParts(lines[0]).Last()[0];
            long steps = long.Parse(GetParts(lines[1])[GetParts(lines[1]).Length - 2]);
            long position = 0;
            var toProcess = new Queue<string>(lines);
            toProcess.Dequeue();
            toProcess.Dequeue();
            var machine = GetMachine(toProcess);
            for (int i = 0; i < steps; i++)
            {
                var step = machine[state];
                bool current = tape.GetValueOrDefault(position);
                if (current)
                {
                    tape[position] = step.Value1;
                    position += step.Offset1;
                    state= step.NewState1;
                }
                else
                {
                    tape[position] = step.Value0;
                    position += step.Offset0;
                    state = step.NewState0;
                }

            }
            return tape.Values.Count(v => v).ToString();
        }

        private Dictionary<char, StateTransition> GetMachine(Queue<string> toProcess)
        {
            var machine = new Dictionary<char, StateTransition>();
            while (toProcess.Any())
            {
                var stateTransition = GetTrans(toProcess.Take(9).ToArray());
                machine[stateTransition.State] = stateTransition;
                for (int i = 0; i < 9; i++)
                {
                    toProcess.Dequeue();
                }

            }
            return machine;
        }

        private StateTransition GetTrans(string[] lines)
        {
            return new StateTransition()
            {
                State = GetParts(lines[0]).Last()[0],
                Value0 = GetParts(lines[2]).Last()[0] == '1' ? true : false,
                Offset0 = GetParts(lines[3]).Last() == "right" ? 1 : -1,
                NewState0 = GetParts(lines[4]).Last()[0],
                Value1 = GetParts(lines[6]).Last()[0] == '1' ? true : false,
                Offset1 = GetParts(lines[7]).Last() == "right" ? 1 : -1,
                NewState1 = GetParts(lines[8]).Last()[0]
            };
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
