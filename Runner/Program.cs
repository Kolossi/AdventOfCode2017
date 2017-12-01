using System;
using System.Collections.Generic;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Day> days = new List<Day>() {new Day01()};
            foreach (var day in days)
            {
                Console.WriteLine(day.GetType().Name);
                Console.WriteLine("Checking");
                day.TestFirst();
                day.TestSecond();
                Console.Write("    First : ");
                Console.WriteLine(day.SolveFirst());
                Console.Write("    Second : ");
                Console.WriteLine(day.SolveSecond());
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}
