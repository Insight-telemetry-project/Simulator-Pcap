using SendRecieveUDP.Model.Interfaces.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendRecieveUDP.Common.Constant;

namespace SendRecieveUDP.Service.Csv
{
    public class CleanCsv : ICsvCleaner
    {
        public void Run(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($" File {inputFile} not found!");
                return;
            }

            string[] lines = File.ReadAllLines(inputFile);
            if (lines.Length == ConstantCsv.EMPTY)
            {
                Console.WriteLine(" CSV is empty!");
                return;
            }

            using var streamWriter = new StreamWriter(outputFile);
            streamWriter.WriteLine(lines[ConstantCsv.TITLE_ROW]);

            for (int rowIndex = ConstantCsv.ONE_LINE; rowIndex < lines.Length; rowIndex++)
            {
                string[] parts = lines[rowIndex].Split(ConstantCsv.CSV_DELIMITER);

                for (int column = ConstantCsv.EMPTY; column < parts.Length; column++)
                {
                    if (parts[column].StartsWith(ConstantCsv.CLUSTER))
                        parts[column] = parts[column].Substring(ConstantCsv.ONLY_TITLES);
                }

                streamWriter.WriteLine(string.Join(ConstantCsv.CSV_DELIMITER, parts));
            }

            Console.WriteLine($" Clean CSV saved to {outputFile}");
        }
    }
}
