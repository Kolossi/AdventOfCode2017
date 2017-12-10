using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day10 : Day
    {
        public override string First(string input)
        {
            var parts = input.Split("-");
            var arrayLength = int.Parse(parts[0]);
            var hashLengths = parts[1].Split(",").Select(l => int.Parse(l)).ToArray();
            var knotHash = Process(hashLengths, arrayLength, k => k.DoHashPass());
            return (knotHash.Data[0] * knotHash.Data[1]).ToString();
        }

        public override string Second(string input)
        {
            var parts = input.Split("-");
            var arrayLength = int.Parse(parts[0]);
            var inputLengths = parts[1].ToArray().Select(c => (byte)c).ToArray();
            int[] hashLengths = AddSalt(inputLengths);

            var knotHash = Process(hashLengths, arrayLength, k=>k.DoHash());
            return knotHash.DenseHash;
        }

        private static int[] AddSalt(byte[] inputLengths)
        {
            var salt = new int[] { 17, 31, 73, 47, 23 };
            var hashLengths = new int[inputLengths.Length + salt.Length];
            Array.Copy(inputLengths, 0, hashLengths, 0, inputLengths.Length);
            Array.Copy(salt, 0, hashLengths, inputLengths.Length, salt.Length);
            return hashLengths;
        }

        private KnotHash Process(int[] hashLengths, int arrayLength,Action<KnotHash> action)
        {

            var array = new int[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                array[i] = i;
            }

            var knotHash = new KnotHash()
            {
                Pos = 0,
                Skip = 0,
                Data = array,
                HashLengths = hashLengths
            };

            action(knotHash);
            return knotHash;
        }

        class KnotHash
        {
            public int[] Data { get; set; }
            public int Pos { get; set; }
            public int Skip { get; set; }
            public int[] HashLengths { get; set; }
            public string DenseHash { get; set; }

            public void DoHash()
            {
                for (int i = 0; i < 64; i++)
                {
                    DoHashPass();
                }

                var denseHash = GetDenseHash();

                DenseHash = string.Join("", denseHash.Select(h => h.ToString("x2")));
            }

            public int[] GetDenseHash()
            {
                int densePos = 0;
                var result = new List<int>();
                var section = new int[16];
                do
                {
                    Array.Copy(Data, densePos, section, 0, 16);
                    result.Add(section.Aggregate((a, b) => a^b));
                    densePos += 16;
                } while (densePos<Data.Length);
                return result.ToArray();
            }

            public void DoHashPass()
            {
                foreach (var hashLength in HashLengths)
                {
                    if (hashLength > Data.Length) continue;
                    var slice = GetCircularSlice(Data, Pos, hashLength);
                    Array.Reverse(slice);
                    InsertCircularSlice(Data, slice, Pos);
                    Pos = (Pos + slice.Length + Skip++) % Data.Length;
                }
            }

            private static int[] GetCircularSlice(int[] array, int startPos, int sliceLength)
            {
                var result = new int[sliceLength];
                var firstSliceLength = Math.Min(sliceLength, array.Length - startPos);
                var secondSliceLength = sliceLength - firstSliceLength;
                Array.Copy(array, startPos, result, 0, firstSliceLength);
                Array.Copy(array, 0, result, firstSliceLength, secondSliceLength);
                return result;
            }

            private static void InsertCircularSlice(int[] array, int[] slice, int startPos)
            {
                var firstSliceLength = Math.Min(slice.Length, array.Length - startPos);
                var secondSliceLength = slice.Length - firstSliceLength;
                Array.Copy(slice, 0, array, startPos, firstSliceLength);
                Array.Copy(slice, firstSliceLength, array, 0, secondSliceLength);
            }
        }
    }
}
