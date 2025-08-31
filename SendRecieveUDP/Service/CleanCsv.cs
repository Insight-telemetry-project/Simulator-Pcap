using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Service
{
    internal class CleanCsv
    {
        public static void Run(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($" File {inputFile} not found!");
                return;
            }

            var lines = File.ReadAllLines(inputFile);
            if (lines.Length == 0)
            {
                Console.WriteLine(" CSV is empty!");
                return;
            }

            using var sw = new StreamWriter(outputFile);
            sw.WriteLine(lines[0]);

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');

                for (int c = 0; c < parts.Length; c++)
                {
                    if (parts[c].StartsWith("c_"))
                        parts[c] = parts[c].Substring(2);
                }

                sw.WriteLine(string.Join(",", parts));
            }

            Console.WriteLine($" Clean CSV saved to {outputFile}");
        }
    }
}
