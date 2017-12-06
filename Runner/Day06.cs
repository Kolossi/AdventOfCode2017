using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day06 :  Day
    {
        class Memory
        {
            public Block[] Blocks { get; set; }

            public Memory(IEnumerable<Block> blocks)
            {
                Blocks = blocks.ToArray();
            }
            public override bool Equals(object obj)
            {
                var other = obj as Memory;
                if (other == null) return false;
                if (other.Blocks.Length!=Blocks.Length) return false;
                var result = !((Blocks.Select((b, i) => b.Equals(other.Blocks[i]))
                    .Any(t => t == false)));
                return result;
            }

            public Memory Clone()
            {
                return new Memory(Blocks.Select(b=>b.Clone()));
            }

            public override string ToString()
            {
                return string.Join(" ",Blocks.Select(b=>b.BlockCount.ToString()));
            }

            public override int GetHashCode()
            {
                return Blocks.Aggregate(29, (i, b) => i ^ b.GetHashCode());
            }
        }
        class Block
        {
            public int Index { get; set; }
            public int BlockCount { get; set; }
            public override bool Equals(object obj)
            {
                var other = obj as Block;
                if (other == null) return false;
                var result = (Index == other.Index && BlockCount == other.BlockCount);
                return result;
            }

            public override int GetHashCode()
            {
                return 59 ^ Index ^ BlockCount;
            }

            public Block Clone()
            {
                return new Block() {BlockCount = BlockCount, Index = Index};
            }

            public override string ToString()
            {
                return string.Format("[{0},{1}]", Index, BlockCount);
            }
        }

        public override string First(string input)
        {
            var memory =
                new Memory(GetParts(input).Select((c, i) => new Block() {Index = i, BlockCount = int.Parse(c)}).ToArray());
            return GetRedistributeCount(memory).ToString();
        }

        private int GetRedistributeCount(Memory startMemory)
        {
            int count = 0;
            var memory = startMemory.Clone();
            var previousStates = new HashSet<Memory>();
            do
            {
                previousStates.Add(memory);
                memory = Redistribute(memory);
                count++;
            } while (!previousStates.Contains(memory));
            return count;
        }

        private int GetLoopSize(Memory startMemory)
        {
            int count = 0;
            var memory = startMemory.Clone();
            var previousStates = new List<Memory>();
            do
            {
                previousStates.Add(memory);
                memory = Redistribute(memory);
                count++;
            } while (!previousStates.Contains(memory));
            return count-(previousStates.IndexOf(memory));
        }

        private Memory Redistribute(Memory memory)
        {
            var newMemory = memory.Clone();
            var largestBlock = memory.Blocks.OrderByDescending(b => b.BlockCount).ThenBy(b => b.Index).First();
            var blocksToAssign = largestBlock.BlockCount;
            var startIndex = largestBlock.Index;
            newMemory.Blocks[startIndex].BlockCount = 0;
            for (int i = 0; i < blocksToAssign; i++)
            {
                var index0 = (i + startIndex + 1) % newMemory.Blocks.Length;
                newMemory.Blocks[index0].BlockCount++;
            }

            return newMemory;

        }

        public override string Second(string input)
        {
            var memory =
                new Memory(GetParts(input).Select((c, i) => new Block() { Index = i, BlockCount = int.Parse(c) }).ToArray());
            return GetLoopSize(memory).ToString();
        }
    }
}
