using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
            var array = GetBitArray(input);
            var groupsArray = GetGroupArray(array);
            Console.WriteLine();
            return GetGroupCount(groupsArray).ToString();
        }

        private static int GetGroupCount(int[,] groupArray)
        {
            var groups = new HashSet<int>();
            for (int y = 0; y < groupArray.GetLength(1); y++)
            {
                for (int x = 0; x < groupArray.GetLength(0); x++)
                {
                    groups.Add(groupArray[x, y]);
                }
            }
            return groups.Count(g=>g!=0);
        }

        private static int[,] GetGroupArray(bool[,] array)
        {
            var groupArray = new int[array.GetLength(0), array.GetLength(1)];
            int nextGroupNum = 1,groupNum=1;
            bool inGroup = false;
            for (int y = 0; y < array.GetLength(1); y++)
            {
                for (int x = 0; x < array.GetLength(0); x++)
                {
                    if (array[x, y])
                    {
                        if (!inGroup)
                        {
                            groupNum = nextGroupNum++;
                        }
                        inGroup = true;
                        groupArray[x, y] = groupNum;
                        if (y > 0 && array[x, y-1])
                        {
                            var previousGroup = groupArray[x, y-1];
                            MergeGroup(groupArray, groupNum, previousGroup, y);
                            groupNum = previousGroup;
                        }
                    }
                    else if (inGroup)
                    {
                        inGroup = false;
                    }
                }
                inGroup = false;
            }

            return groupArray;
        }

        private static void MergeGroup(int[,] groupArray, int groupNum, int previousGroup, int row)
        {
            for (int y = 0; y < groupArray.GetLength(1); y++)
            {
                for (int x = 0; x < groupArray.GetLength(0); x++)
                {
                    if (groupArray[x, y] == groupNum) groupArray[x, y] = previousGroup;
                }
            }
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

        //private string ShowBits(bool[,] array)
        //{
        //    var sb = new StringBuilder();
        //    for (int x = 0; x < array.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < array.GetLength(1); y++)
        //        {
        //            sb.Append(array[x, y] ? "1" : "0");
        //        }
        //        sb.AppendLine();
        //    }
        //    return sb.ToString();
        //}

        //private string ShowGroups(int[,] array)
        //{
        //    var sb = new StringBuilder();
        //    for (int x = 0; x < array.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < array.GetLength(1); y++)
        //        {
        //            sb.Append(string.Format("{0:D3} ",array[x, y]));
        //        }
        //        sb.AppendLine();
        //    }
        //    return sb.ToString();
        //}

        private void SetBits(bool[,] array, int row, int[] hash)
        {
            var binary = string.Join("", hash.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToArray());
            for (int i = 0; i < binary.Length; i++)
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
                var subTotal = CountBits(knotHash.DenseHash);
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
            var inputBytes = input.ToArray().Select(c => (byte)c).ToArray();
            int[] bytes = AddSalt(inputBytes);

            var knotHash = Process(bytes);
            return knotHash;

        }
        private KnotHash Process(int[] bytes)
        {
            int arrayLength = 256;
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
                HashLengths = bytes
            };

            knotHash.DoHash();
            return knotHash;
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
