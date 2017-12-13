using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    class Day13 : Day
    {
        class Scanner
        {
            public int Depth { get; set; }
            public int Range { get; set; }
        }

        public override string First(string input)
        {
            var scanners=GetScanners(input);
            var severity = GetSeverity(scanners);
            return severity.ToString();
        }

        private int GetSeverity(List<Scanner> scanners, int delay=0)
        {
            int severity = 0;
            for (int i = 0; i <= scanners.Max(s=>s.Depth); i++)
            {
                var scanner = scanners.FirstOrDefault(s => s.Depth == i);
                if (scanner != null && IsCaught(scanner, i+delay))
                {
                    severity += scanner.Depth * scanner.Range;
                }
            }
            return severity;
        }

        private bool IsCaught(List<Scanner> scanners, int delay = 0)
        {
            for (int i = 0; i <= scanners.Max(s => s.Depth); i++)
            {
                var scanner = scanners.FirstOrDefault(s => s.Depth == i);
                if (scanner != null && IsCaught(scanner, i + delay))
                {
                    return true;
                }
            }
            return false;
        }


        private bool IsCaught(Scanner scanner, int time)
        {
            return (GetPosition(scanner, time) == 0);
        }

        private int GetPosition(Scanner scanner, int time)
        {
            int strokeLength = scanner.Range * 2 - 2;
            int pos = time % strokeLength;
            if (pos >= scanner.Range)
            {
                pos = scanner.Range - 2 - pos % scanner.Range;

            }
            return pos;

        }

        private List<Scanner> GetScanners(string input)
        {
            var scanners = new List<Scanner>();

            var lines = GetLines(input);
            foreach (var line in lines)
            {
                var parts = line.Split(", ");
                scanners.Add(new Scanner()
                {
                    Depth = int.Parse(parts[0]),
                    Range = int.Parse(parts[1])
                });
            }
            return scanners;
        }

        public override string Second(string input)
        {
            //return SecondWithParallel(input);
            var scanners = GetScanners(input);
            int delay = 0;
            do
            {
                if (!IsCaught(scanners, delay))
                {
                    return delay.ToString();
                }
                delay++;
            } while (true);
        }

        // reduced solution from 10 secs to 6 secs
        public string SecondWithParallel(string input)
        {
            int batchSize = 1000000;
            var scanners = GetScanners(input);
            int delayOffset = 0;
            int result = 0;
            do
            {
                Parallel.ForEach(Enumerable.Range(delayOffset, batchSize),
                    d =>
                    {
                        if (result > 0) return;
                        int delay = (int) d;
                        if (!IsCaught(scanners, delay))
                        {
                            result=delay;
                            return;
                        }
                    });
                if (result > 0) break;
                delayOffset += batchSize;
            } while (true);
            return result.ToString();
        }
    }
}
