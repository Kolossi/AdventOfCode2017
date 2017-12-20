﻿using System;

namespace Percentiler
{
    class Program
    {
        ////////////////////////////////////////////////
        // paste from http://adventofcode.com/2017/leaderboard/self
        ////////////////////////////////////////////////
        /// 
        /// 
        private static string PersonalStats = @"      -------Part 1--------   -------Part 2--------
Day       Time  Rank  Score       Time  Rank  Score
 18   02:46:10  1502      0   05:45:32  1264      0
 17   03:46:34  1948      0   04:02:13  1635      0
 16   03:35:33  1729      0   05:15:54  1549      0
 15   02:18:32  1765      0   02:40:16  1742      0
 14   02:51:01  1798      0   04:48:34  1629      0
 13   04:15:47  2733      0   04:45:38  2062      0
 12   01:19:48  1552      0   01:40:13  1528      0
 11   03:55:22  2405      0   03:57:16  2207      0
 10   03:48:07  1945      0   04:43:18  1730      0
  9   04:59:23  2698      0   05:06:44  2642      0
  8   01:48:29  2039      0   01:58:06  2072      0
  7   01:22:44  2057      0   03:52:52  1881      0
  6   03:19:54  3482      0   03:43:21  3547      0
  5   02:47:13  3764      0   02:52:58  3610      0
  4   03:04:19  3659      0   03:59:23  3800      0
  3   07:26:22  4676      0   14:16:27  5505      0
  2   08:24:26  7789      0   08:41:36  6584      0
  1   05:05:06  4229      0   05:27:18  3576      0";



        ////////////////////////////////////////////////
        // paste from http://adventofcode.com/2017/stats
        ////////////////////////////////////////////////
        /// 
        /// 
        private static string GlobalStats = @"25      0     0  
24      0     0  
23      0     0  
22      0     0  
21      0     0  
20      0     0  
19      0     0  
18   2187  1449  *****
17   5226   423  ******
16   5795   730  *******
15   6828   194  ********
14   6153   605  *******
13   7511   771  *********
12   8708   354  **********
11   9089   337  **********
10   9341  1035  **********
 9  11340   255  ************
 8  13195   325  **************
 7  12077  3798  ****************
 6  17086   742  ******************
 5  19623  1033  ********************
 4  21176  2261  ***********************
 3  18320  6456  *************************
 2  30151  5174  **********************************
 1  35505  7019  *****************************************";

        private static Tuple<int, int>[] PersonalPlacings;

        private static Tuple<int, int>[] GlobalTotals;

        static void Main(string[] args)
        {
            PersonalPlacings = new Tuple<int, int>[25];
            GlobalTotals = new Tuple<int, int>[25];

            foreach (var personalStat in PersonalStats.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = personalStat.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                int day;
                if (!int.TryParse(parts[0], out day)) continue;
                PersonalPlacings[day-1] = new Tuple<int, int>(int.Parse(parts[2]), int.Parse(parts[5]));
            }

            foreach (var globalStat in GlobalStats.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = globalStat.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                int day;
                if (!int.TryParse(parts[0], out day) || day==0) continue;
                GlobalTotals[day-1] = new Tuple<int, int>(int.Parse(parts[1]) + int.Parse(parts[2]), int.Parse(parts[1]));
            }

            var day1Part1 = GlobalTotals[0].Item1;

            for (int i = 24; i >= 0; i--)
            {
                if (PersonalPlacings[i] == null || GlobalTotals[i] == null) continue;
                Console.WriteLine("{0,5}  {1,5}/{2,5} = {3:f0}% of day,   {4:f0}% of all       {5,5}/{6,5} = {7:f0}% of day,   {4:f0}% of all", i+1,
                    PersonalPlacings[i].Item1, GlobalTotals[i].Item1,
                    100.0F * (1.0F - (float)PersonalPlacings[i].Item1 / (float) GlobalTotals[i].Item1),
                    100.0F * (1.0F - (float)PersonalPlacings[i].Item1 / (float) day1Part1),
                    PersonalPlacings[i].Item2, GlobalTotals[i].Item2,
                    100.0F * (1.0F - (float)PersonalPlacings[i].Item2 / (float) GlobalTotals[i].Item2),
                    100.0F * (1.0F - (float)PersonalPlacings[i].Item2 / (float) day1Part1));
            }

            Console.WriteLine();
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}