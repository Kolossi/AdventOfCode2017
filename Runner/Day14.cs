using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day14 : Day
    {
        public override string First(string input)
        {
            return CountDefragBits(input).ToString();
        }

        public override string Second(string input)
        {
            return ShowBits(GetBitArray(input));
        }

        private bool[,] GetBitArray(string input)
        {
            var array = new bool[128,128];
            for (int i = 0; i < 128; i++)
            {
                var knotHash = GetKnotHash(string.Format("{0}-{1}", input, i));
                SetBits(array,i,knotHash.DenseHash);
            }

            return array;
        }

        private string ShowBits(bool[,] array)
        {
            var sb = new StringBuilder();
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    sb.Append(array[x, y] ? "1" : "0");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void SetBits(bool[,] array, int row, int[] hash)
        {
            var binary = string.Join("",hash.Select(b => Convert.ToString(b, 2).PadLeft(8,'0')).ToArray());
            for (int i = 0; i < binary.Count(); i++)
            {
                array[row, i] = binary[i] == '1' ? true : false;
            }
        }

        private int CountDefragBits(string input)
        {
            int total = 0;
            for (int i = 0; i < 128; i++)
            {
                var knotHash = GetKnotHash(string.Format("{0}-{1}", input,i));
                //Console.WriteLine(knotHash.DenseHash);
                var subTotal = CountBits(knotHash.DenseHash);
                //Console.WriteLine(subTotal);
                total += subTotal;
            }

            return total;
        }

        private int CountBits(int[] bytes)
        {
            var binary = bytes.Select(b => Convert.ToString(b, 2));
            var oneCount = binary.Sum(b => b.Sum(c=> c == '1'?1:0));
            return oneCount;
        }

        private KnotHash GetKnotHash(string input)
        {
            var arrayLength = 256;
            var inputLengths = input.ToArray().Select(c => (byte)c).ToArray();
            int[] hashLengths = AddSalt(inputLengths);

            var knotHash = Process(hashLengths, arrayLength, k => k.DoHash());
            return knotHash;

        }
        private KnotHash Process(int[] hashLengths, int arrayLength, Action<KnotHash> action)
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

        public static int[] AddSalt(byte[] inputLengths, int[] salt)
        {
            var hashLengths = new int[inputLengths.Length + salt.Length];
            Array.Copy(inputLengths, 0, hashLengths, 0, inputLengths.Length);
            Array.Copy(salt, 0, hashLengths, inputLengths.Length, salt.Length);
            return hashLengths;
        }

        private static int[] AddSalt(byte[] inputLengths)
        {
            var salt = new int[] { 17, 31, 73, 47, 23 };
            var hashLengths = new int[inputLengths.Length + salt.Length];
            Array.Copy(inputLengths, 0, hashLengths, 0, inputLengths.Length);
            Array.Copy(salt, 0, hashLengths, inputLengths.Length, salt.Length);
            return hashLengths;
        }

        // from Day 10
        public class KnotHash
        {
            public int[] Data { get; set; }
            public int Pos { get; set; }
            public int Skip { get; set; }
            public int[] HashLengths { get; set; }
            public int[] DenseHash { get; set; }

            public void DoHash()
            {
                for (int i = 0; i < 64; i++)
                {
                    DoHashPass();
                }

                DenseHash = GetDenseHash();

                //DenseHash = string.Join("", denseHash.Select(h => h.ToString("x2")));
            }

            public int[] GetDenseHash()
            {
                int densePos = 0;
                var result = new List<int>();
                var section = new int[16];
                do
                {
                    Array.Copy(Data, densePos, section, 0, 16);
                    result.Add(section.Aggregate((a, b) => a ^ b));
                    densePos += 16;
                } while (densePos < Data.Length);
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
