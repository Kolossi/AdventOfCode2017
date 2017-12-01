using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Runner
{
    public abstract class Day
    {
        public abstract string First(string input);
        public abstract string Second(string input);

        public string Solve(string set, Func<string,string> solver)
        {
            string input;
            try
            {
                GetInput(set);
            }
            catch (FileNotFoundException e)
            {
                return "INPUT MISSING";
            }

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var result =  solver(GetInput(set));
                sw.Stop();
                Console.WriteLine("Took : {0} ticks / {1} ms",sw.ElapsedTicks,sw.ElapsedMilliseconds);
                return result;
            }
            catch (NotImplementedException e)
            {
                return "NOT IMPLEMENTED";
            }
        }

        public string SolveFirst()
        {
            return Solve("First", this.First);
        }

        public string SolveSecond()
        {
            return Solve("Second", this.Second);
        }

        public bool TestFirst()
        {
            return Test("First", this.First);
        }

        public bool TestSecond()
        {
            return Test("Second", this.Second);
        }

        public bool Test(string set, Func<string, string> solver)
        {
            bool result = true;
            string input = GetInput(string.Format("{0}Tests",set));
            var lines = input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine(string.Format("{0} Tests", set));
            foreach (var line in lines)
            {
                Console.Write(string.Format("  {0} : ",line));
                var parts = line.Split(":");
                var testInput = parts[0];
                var expectedOutput = parts[1];
                string output=string.Empty;
                try
                {
                    output = solver(testInput);
                }
                catch (NotImplementedException e)
                {
                    Console.WriteLine("NOT IMPLEMENTED");
                    result = false;
                }
                
                if (output == expectedOutput)
                {
                    Console.WriteLine("OK");
                }
                else
                {
                    result = false;
                    Console.WriteLine(string.Format("    Input : {0}, Expected : {1}, Got : {2}", testInput,
                        expectedOutput, output));
                }

            }
            return result;
        }

        public string GetInput(string set)
        {
            string filename = string.Format("Inputs/{0}{1}.txt",this.GetType().Name, set);
            string input = System.IO.File.ReadAllText(filename);
            return input;
        }
    }
}
